using Foodtruck.Shared.Customers;
using Foodtruck.Shared.Formulas;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Reservations;
using Foodtruck.Shared.Supplements;

namespace Services.Tests.Pdfs.Helpers;

public static class QuotationCalculatorTestData
{
	public static IEnumerable<object[]> VatPercentagesData
	{
		get
		{
			// 21% BTW
			yield return new object[] {
				new QuotationSupplementLineDto()
				{
					Id = 1,
					Name = "foo",
					Description = "bar",
					SupplementPrice = 500M,
					SupplementVat = 105M,
					Quantity = 1
				},
				21
			};

			// 6% BTW
			yield return new object[] {
				new QuotationSupplementLineDto()
				{
					Id = 1,
					Name = "foo",
					Description = "bar",
					SupplementPrice = 50.39M,
					SupplementVat = 3.0234M,
					Quantity = 1
				},
				6
			};

			// 0% BTW
			yield return new object[] {
				new QuotationSupplementLineDto()
				{
					Id = 1,
					Name = "foo",
					Description = "bar",
					SupplementPrice = 22.55M,
					SupplementVat = 0M,
					Quantity = 1
				},
				0
			};

			// 100% BTW :)
			yield return new object[] {
				new QuotationSupplementLineDto()
				{
					Id = 1,
					Name = "foo",
					Description = "bar",
					SupplementPrice = 22.55M,
					SupplementVat = 22.55M,
					Quantity = 1
				},
				100
			};
		}
	}

	public static QuotationDto.Detail quotationDtoDetail => new QuotationDto.Detail()
	{
		Id = 1,
		Customer = new CustomerDto.Detail()
		{
			Firstname = "foo",
			Lastname = "bar",
			Email = "foo@bar.com",
		},
	};

	public static QuotationVersionDto.Detail quotationVersionDtoDetail => new QuotationVersionDto.Detail()
	{
		Id = 1,
		VersionNumber = 1,
		NumberOfGuests = 10,
		ExtraInfo = "this is the extra info",
		Description = "bar",
		Formula = new FormulaDto.Detail()
		{
			Id = 1,
			Choices = new List<FormulaSupplementChoiceDto.Detail>()
				{
					new FormulaSupplementChoiceDto.Detail()
					{
						Id = 1,
						Name = "foo",
						MinQuantity = 2,
						DefaultChoice = new SupplementDto.Detail()
						{
							Name = "bar",
							AmountAvailable = 3,
							Category = new CategoryDto.Index()
							{
								Id = 1,
								Name = "foo",
							},
							CreatedAt = DateTime.Now,
							Description = "bar",
							Id = 1,
							ImageUrls = new List<Uri>()
							{
								new Uri("https://i.kym-cdn.com/entries/icons/facebook/000/000/091/TrollFace.jpg")
							},
							Price = 20,
							UpdatedAt = DateTime.Now,
						},
						SupplementsToChoose = new List<SupplementDto.Detail>()
						{
							new SupplementDto.Detail()
							{
								Name = "bar",
								AmountAvailable = 3,
								Category = new CategoryDto.Index()
								{
									Id = 1,
									Name = "foo",
								},
								CreatedAt = DateTime.Now,
								Description = "bar",
								Id = 1,
								ImageUrls = new List<Uri>()
								{
									new Uri("https://i.kym-cdn.com/entries/icons/facebook/000/000/091/TrollFace.jpg")
								},
								Price = 20,
								UpdatedAt = DateTime.Now,
							}
						}
					}
				},
			Title = "bar",
			CreatedAt = DateTime.Now,
			Description = "description of the formula",
			ImageUrl = new Uri("https://i.kym-cdn.com/entries/icons/facebook/000/000/091/TrollFace.jpg"),
			UpdatedAt = DateTime.Now,
			IncludedSupplements = new List<FormulaSupplementLineDto.Detail>()
				{
					new FormulaSupplementLineDto.Detail()
					{
						Id = 1,
						Quantity = 3,
						Supplement = new SupplementDto.Detail()
						{
							Name = "bar",
							AmountAvailable = 3,
							Category = new CategoryDto.Index()
							{
								Id = 1,
								Name = "foo",
							},
							CreatedAt = DateTime.Now,
							Description = "bar",
							Id = 1,
							ImageUrls = new List<Uri>()
							{
								new Uri("https://i.kym-cdn.com/entries/icons/facebook/000/000/091/TrollFace.jpg")
							},
							Price = 20,
							UpdatedAt = DateTime.Now,
						}
					}
				}
		},
		Reservation = new ReservationDto.Detail()
		{
			Description = "bar",
			Start = DateTime.Now,
			End = DateTime.Now.AddDays(3),
			Id = 1,
			Status = StatusDto.Voorgesteld
		},
		BillingAddress = new AddressDto()
		{
			Street = "random street",
			City = "random city",
			HouseNumber = "21",
			Zip = "420",
		},

		EventAddress = new AddressDto()
		{
			Street = "random street",
			City = "random city",
			HouseNumber = "21",
			Zip = "420",
		},
		FoodtruckPrice = 1000M,
		Price = 1000M,
		VatTotal = 210M,
		FormulaSupplementLines = new List<QuotationSupplementLineDto>()
			{
				new QuotationSupplementLineDto()
				{
					Description = "bar",
					Id = 1,
					Name = "foo",
					Quantity = 3,
					SupplementPrice = 20,
					SupplementVat = 4.2M,
				},
				new QuotationSupplementLineDto()
				{
					Description = "another product",
					Id = 1,
					Name = "another name",
					Quantity = 3,
					SupplementPrice = 300,
					SupplementVat = 4.2M,
				}
			}

	};

	public static List<QuotationSupplementLineDto> quotationSupplementLineDtos = new List<QuotationSupplementLineDto>()
			{
				new QuotationSupplementLineDto()
				{
					Description = "bar",
					Id = 1,
					Name = "foo",
					Quantity = 3,
					SupplementPrice = 20,
					SupplementVat = 4.2M,
				},
				new QuotationSupplementLineDto()
				{
					Description = "another product",
					Id = 1,
					Name = "another name",
					Quantity = 3,
					SupplementPrice = 300,
					SupplementVat = 63M,
				}
			};

}
