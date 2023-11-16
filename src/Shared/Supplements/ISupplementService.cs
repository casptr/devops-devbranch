namespace Foodtruck.Shared.Supplements;

public interface ISupplementService
{
    Task<SupplementResult.Index> GetAllAsync();
    Task<SupplementResult.Index> GetIndexAsync(SupplementRequest.Index request);
    Task<SupplementDto.Detail> GetDetailAsync(int supplementId);
    Task<SupplementResult.Mutate> CreateAsync(SupplementDto.Mutate model);
    Task<SupplementResult.Mutate> EditAsync(int supplementId, SupplementDto.Mutate model);
    Task DeleteAsync(int supplementId); 

    //Task<int> AddImage(int supplementId, SupplementDto.Mutate model);
}
