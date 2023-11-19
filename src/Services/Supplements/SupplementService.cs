using Domain.BlobFiles;
using Domain.Exceptions;
using Domain.Supplements;
using Foodtruck.Persistence;
using Foodtruck.Shared.Supplements;
using Microsoft.EntityFrameworkCore;
using Services.BlobFiles;

namespace Services.Supplements;
public class SupplementService : ISupplementService
{

	private readonly FoodtruckDbContext dbContext;
	private readonly IStorageService storageService;

	public SupplementService(FoodtruckDbContext dbContext, IStorageService storageService)
	{
		this.dbContext = dbContext;
		this.storageService = storageService;
	}

	// GET all
	public async Task<SupplementResult.Index> GetAllAsync()
	{
		var query = dbContext.Supplements.AsQueryable();
		query = dbContext.Supplements;
		int totalAmount = await query.CountAsync();
		var items = await query
		  .OrderBy(x => x.Id)
		  .Select(x => new SupplementDto.Detail
		  {
			  Id = x.Id,
			  Name = x.Name,
			  Price = x.Price.Value,
			  Description = x.Description,
			  Category = new CategoryDto.Index { Id = x.Category.Id, Name = x.Category.Name },
			  ImageUrls = x.ImageUrls.ToList().Select(image => image.Image)!,
			  AmountAvailable = x.AmountAvailable,
			  CreatedAt = x.CreatedAt,
			  UpdatedAt = x.UpdatedAt
		  }).ToListAsync();

		var result = new SupplementResult.Index
		{
			Supplements = items,
			TotalAmount = totalAmount
		};

		return result;
	}

	// GET detail
	public async Task<SupplementDto.Detail> GetDetailAsync(int supplementId)
	{
		SupplementDto.Detail? supplement = await dbContext.Supplements.Select(x => new SupplementDto.Detail
		{
			Id = x.Id,
			Name = x.Name,
			Price = x.Price.Value,
			Description = x.Description,
			Category = new CategoryDto.Index { Id = x.Category.Id, Name = x.Category.Name },
			ImageUrls = x.ImageUrls.Select(image => image.Image)!,
			AmountAvailable = x.AmountAvailable,
			CreatedAt = x.CreatedAt,
			UpdatedAt = x.UpdatedAt
		}).SingleOrDefaultAsync(x => x.Id == supplementId);

		if (supplement is null)
			throw new EntityNotFoundException(nameof(Supplement), supplementId);

		return supplement;
	}

	// GET index
	public async Task<SupplementResult.Index> GetIndexAsync(SupplementRequest.Index request)
	{
		var query = dbContext.Supplements.AsQueryable();

		if (!string.IsNullOrWhiteSpace(request.Searchterm))
			query = query.Where(s => s.Name.Contains(request.Searchterm, StringComparison.OrdinalIgnoreCase));

		if (request.MinPrice is not null)
			query = query.Where(x => x.Price.Value >= request.MinPrice);

		if (request.MaxPrice is not null)
			query = query.Where(x => x.Price.Value <= request.MaxPrice);

		if (request.MinAvailableAmount is not null)
			query = query.Where(x => x.AmountAvailable >= request.MinAvailableAmount);

		if (request.MaxAvailableAmount is not null)
			query = query.Where(x => x.AmountAvailable <= request.MaxAvailableAmount);

		if (!string.IsNullOrWhiteSpace(request.Category))
			query = query.Where(s => s.Category.Name.Equals(request.Category, StringComparison.OrdinalIgnoreCase));

		int totalAmount = await query.CountAsync();

		var items = await query
		   .Skip((request.Page - 1) * request.PageSize)
		   .Take(request.PageSize)
		   .OrderBy(x => x.Id)
		   .Select(x => new SupplementDto.Detail
		   {
			   Id = x.Id,
			   Name = x.Name,
			   Price = x.Price.Value,
			   Description = x.Description,
			   Category = new CategoryDto.Index { Id = x.Category.Id, Name = x.Category.Name },
			   ImageUrls = x.ImageUrls.Select(image => image.Image)!,
			   AmountAvailable = x.AmountAvailable,
			   CreatedAt = x.CreatedAt,
			   UpdatedAt = x.UpdatedAt
		   }).ToListAsync();

		var result = new SupplementResult.Index
		{
			Supplements = items,
			TotalAmount = totalAmount
		};
		return result;
	}

	// POST
	public async Task<SupplementResult.Mutate> CreateAsync(SupplementDto.Mutate model)
	{
		if (await dbContext.Supplements.AnyAsync(x => x.Name == model.Name))
			throw new EntityAlreadyExistsException(nameof(Supplement), nameof(Supplement.Name), model.Name);

		Supplement supplement = new(
			model.Name!,
			model.Description!,
			new Category(model.Category!.Name!, 16),
			new(model.Price),
			model.AmountAvailable
		);

		var result = SetUploadSasUris(supplement, model);

		dbContext.Supplements.Add(supplement);
		await dbContext.SaveChangesAsync();

		result.SupplementId = supplement.Id;
		return result;
	}

	// PUT
	public async Task<SupplementResult.Mutate> EditAsync(int supplementId, SupplementDto.Mutate model)
	{
		Supplement? supplement = await dbContext.Supplements
			.Include(x => x.ImageUrls)
			.SingleOrDefaultAsync(x => x.Id == supplementId);

		if (supplement is null)
			throw new EntityNotFoundException(nameof(Supplement), supplementId);

		supplement.Name = model.Name!;
		supplement.Description = model.Description!;
		supplement.Category = new Category(model.Category?.Name!, 16);
		supplement.Price = new(model.Price);
		supplement.AmountAvailable = model.AmountAvailable;

		var result = SetUploadSasUris(supplement, model);

		await DeleteImages(supplement, model.ImagesToDelete);
		await dbContext.SaveChangesAsync();

		result.SupplementId = supplement.Id;
		return result;
	}

	// DELETE 
	public async Task DeleteAsync(int supplementId)
	{
		Supplement? supplement = await dbContext.Supplements
			.Include(x => x.ImageUrls)
			.SingleOrDefaultAsync(x => x.Id == supplementId);

		if (supplement is null)
			throw new EntityNotFoundException(nameof(Supplement), supplementId);

		await DeleteImages(supplement, supplement.ImageUrls.Select(x => x.Image!));

		dbContext.Supplements.Remove(supplement);
		await dbContext.SaveChangesAsync();
	}

	// Helpers
	public SupplementResult.Mutate SetUploadSasUris(Supplement supplement, SupplementDto.Mutate model)
	{
		SupplementResult.Mutate result = new()
		{
			UploadSasUris = new List<Uri>(),
		};

		// Generate SasUri for blob upload via Client
		if (model.ImagesToUploadTypes != null)
			foreach (var contentType in model.ImagesToUploadTypes)
			{
				Image image = new(storageService.BasePath, contentType);
				result.UploadSasUris.Add(storageService.GenerateImageUploadSas(image));
				supplement.AddImageUrl(image.FileUri);
			}

		return result;
	}

	public async Task DeleteImages(Supplement supplement, IEnumerable<Uri>? imagesToDelete)
	{
		// Delete immediately via Server
		if (imagesToDelete != null)
			foreach (var imageUrl in imagesToDelete)
			{
				await storageService.DeleteBlobFile(imageUrl);
				var image = supplement.GetSupplementImage(imageUrl);
				if (image is not null) dbContext.SupplementImages.Remove(image);
			}
	}


	//image test
	//public async Task AddImage(int supplementId, SupplementDto.Mutate model)
	//{
	//    Supplement? supplement = await dbContext.Supplements.SingleOrDefaultAsync(x => x.Id == supplementId);

	//    if (supplement is null)
	//        throw new EntityNotFoundException(nameof(Supplement), supplementId);

	//    Faker faker = new();
	//    var image = new Uri(faker.Image.PicsumUrl());
	//    supplement.AddImageUrl(image);
	//    dbContext.Entry(supplement).State = EntityState.Modified;
	//    int update = await dbContext.SaveChangesAsync();
	//}
}
