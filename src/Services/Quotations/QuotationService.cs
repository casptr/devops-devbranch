using Domain.Common;
using Domain.Customers;
using Domain.Exceptions;
using Domain.Formulas;
using Domain.Quotations;
using Domain.Supplements;
using Foodtruck.Persistence;
using Foodtruck.Shared.Customers;
using Foodtruck.Shared.Formulas;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Reservations;
using Foodtruck.Shared.Supplements;
using Microsoft.EntityFrameworkCore;

namespace Services.Quotations
{
    public class QuotationService : IQuotationService
    {
        private readonly FoodtruckDbContext dbContext;

        public QuotationService(FoodtruckDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<QuotationResult.Index> GetIndexAsync(QuotationRequest.Index request)
        {
            var query = dbContext.Quotations.AsQueryable();

            int totalAmount = await query.CountAsync();

            var items = await query
           .Skip((request.Page - 1) * request.PageSize)
           .Take(request.PageSize)
           .OrderBy(x => x.Id)
           .Select(x =>
           new QuotationDto.Index()
           {
               Id = x.Id,
               Customer = new CustomerDto.Detail()
               {
                   Firstname = x.Customer.Firstname,
                   Lastname = x.Customer.Lastname,
                   Email = x.Customer.Email.Value,
                   Phone = x.Customer.Phone,
                   CompanyName = x.Customer.CompanyName,
                   CompanyNumber = x.Customer.CompanyNumber,
               },
               LastQuotationVersion = x.Versions.Select(version => new QuotationVersionDto.Index()
               {
                   Id = version.Id,
                   VersionNumber = version.VersionNumber,
                   NumberOfGuests = version.NumberOfGuests,
                   ExtraInfo = version.ExtraInfo,
                   Description = version.Description,
                   Reservation = new ReservationDto.Detail()
                   {
                       Id = version.Reservation.Id,
                       Description = version.Reservation.Description,
                       Start = version.Reservation.Start,
                       End = version.Reservation.End,
                       Status = (StatusDto)((int)version.Reservation.Status),
                   },
                   Formula = new FormulaDto.Index()
                   {
                       Id = version.Formula.Id,
                       Title = version.Formula.Title,
                   },
                   EventAddress = new AddressDto()
                   {
                       Street = version.EventAddress.Street,
                       City = version.EventAddress.City,
                       HouseNumber = version.EventAddress.HouseNumber,
                       Zip = version.EventAddress.Zip
                   },
                   BillingAddress = new AddressDto()
                   {
                       Street = version.BillingAddress.Street,
                       City = version.BillingAddress.City,
                       Zip = version.BillingAddress.Zip,
                       HouseNumber = version.BillingAddress.HouseNumber,
                   },
                   FoodtruckPrice = version.FoodtruckPrice.Value,
                   Price = version.Price.Value,
                   VatTotal = version.VatTotal.Value,
               }).OrderBy(x => x.Id).Last()
           }).ToListAsync();

            var result = new QuotationResult.Index
            {
                Quotations = items,
                TotalAmount = totalAmount
            };
            return result;
        }

        public async Task<int> CreateAsync(QuotationDto.Create model)
        {
            Customer customer = new Customer(model.Customer.Firstname, model.Customer.Lastname, new EmailAddress(model.Customer.Email), model.Customer.Phone, model.Customer.CompanyName, model.Customer.CompanyNumber);

            Formula? formula = await dbContext.Formulas.Include(x => x.Foodtruck.PricePerDays).SingleOrDefaultAsync(x => x.Id == model.QuotationVersion.FormulaId);

            if (formula is null)
                throw new EntityNotFoundException(nameof(Formula), model.QuotationVersion.FormulaId);


            IEnumerable<SupplementItemDto.Create> allSupplementItemDtos = model.QuotationVersion.ExtraSupplementItems.Concat(model.QuotationVersion.FormulaSupplementItems);

            IEnumerable<Supplement> supplements = await dbContext.Supplements
                                               .Where(x => allSupplementItemDtos.Select(x => x.SupplementId).Contains(x.Id)).Include(x => x.Category)
                                               .ToListAsync();

    

            List<SupplementItem> formulaSupplementItems = new();
            List<SupplementItem> extraSupplementItems = new();

            foreach (var item in model.QuotationVersion.FormulaSupplementItems)
            {
                Supplement? s = supplements.FirstOrDefault(x => x.Id == item.SupplementId);
                if (s is null)
                    throw new EntityNotFoundException(nameof(Supplement), item.SupplementId);

                formulaSupplementItems.Add(new SupplementItem(s, item.Quantity));
            }


            foreach (var item in model.QuotationVersion.ExtraSupplementItems)
            {
                Supplement? s = supplements.FirstOrDefault(x => x.Id == item.SupplementId);
                if (s is null)
                    throw new EntityNotFoundException(nameof(Supplement), item.SupplementId);

                extraSupplementItems.Add(new SupplementItem(s, item.Quantity));
            }


            Address eventAddress = new Address(model.QuotationVersion.EventAddress.Zip, model.QuotationVersion.EventAddress.City, model.QuotationVersion.EventAddress.Street, model.QuotationVersion.EventAddress.HouseNumber);
            Address billingAddress = new Address(model.QuotationVersion.BillingAddress.Zip, model.QuotationVersion.BillingAddress.City, model.QuotationVersion.BillingAddress.Street, model.QuotationVersion.BillingAddress.HouseNumber);
            string reservationDescription = model.Customer.Firstname + " " + model.Customer.Lastname;
            Reservation reservation = new Reservation(model.QuotationVersion.Reservation.Start.Value, model.QuotationVersion.Reservation.End.Value,  reservationDescription);
            QuotationVersion quotationVersion = new QuotationVersion(model.QuotationVersion.NumberOfGuests, model.QuotationVersion.ExtraInfo, "No description", reservation, formula, formulaSupplementItems, extraSupplementItems, eventAddress, billingAddress);


            Quotation newQuotation = new Quotation(customer);
            newQuotation.AddVersion(quotationVersion);

            dbContext.Quotations.Add(newQuotation);
            await dbContext.SaveChangesAsync();

            return newQuotation.Id;
        }


        public async Task<QuotationDto.Detail> GetDetailAsync(int quotationId)
        {
            QuotationDto.Detail? quotation = await dbContext.Quotations.Select(x => new QuotationDto.Detail()
            {
                Id = x.Id,
                Customer = new CustomerDto.Detail()
                {
                    Firstname = x.Customer.Firstname,
                    Lastname = x.Customer.Lastname,
                    Email = x.Customer.Email.Value,
                    Phone = x.Customer.Phone,
                    CompanyName = x.Customer.CompanyName,
                    CompanyNumber = x.Customer.CompanyNumber,
                },
                QuotationVersions = x.Versions.Select(version => new QuotationVersionDto.Detail()
                {
                    Id = version.Id,
                    VersionNumber = version.VersionNumber,
                    NumberOfGuests = version.NumberOfGuests,
                    ExtraInfo = version.ExtraInfo,
                    Description = version.Description,
                    Reservation = new ReservationDto.Detail()
                    {
                        Id = version.Reservation.Id,
                        Description = version.Reservation.Description,
                        Start = version.Reservation.Start,
                        End = version.Reservation.End,
                        Status = (StatusDto)((int)version.Reservation.Status),
                    },
                    Formula = new FormulaDto.Detail()
                    {
                        Id = version.Formula.Id,
                        Title = version.Formula.Title,
                    },
                    EventAddress = new AddressDto()
                    {
                        Street = version.EventAddress.Street,
                        City = version.EventAddress.City,
                        HouseNumber = version.EventAddress.HouseNumber,
                        Zip = version.EventAddress.Zip
                    },
                    BillingAddress = new AddressDto()
                    {
                        Street = version.BillingAddress.Street,
                        City = version.BillingAddress.City,
                        Zip = version.BillingAddress.Zip,
                        HouseNumber = version.BillingAddress.HouseNumber,
                    },
                    FoodtruckPrice = version.FoodtruckPrice.Value,
                    Price = version.Price.Value,
                    VatTotal = version.VatTotal.Value,

                    FormulaSupplementLines = version.QuotationSupplementLines.Where(quotationSupplementLine => quotationSupplementLine.IsIncludedInFormula).Select(quotationSupplementLine => new QuotationSupplementLineDto()
                    {
                        Id = quotationSupplementLine.Id,
                        Description = quotationSupplementLine.Description,
                        Name = quotationSupplementLine.Name,
                        Quantity = quotationSupplementLine.Quantity,
                        SupplementPrice = quotationSupplementLine.SupplementPrice.Value,
                        SupplementVat = quotationSupplementLine.SupplementVat.Value,
                    }),

                    ExtraSupplementLines = version.QuotationSupplementLines.Where(quotationSupplementLine => !quotationSupplementLine.IsIncludedInFormula).Select(quotationSupplementLine => new QuotationSupplementLineDto()
                    {
                        Id = quotationSupplementLine.Id,
                        Description = quotationSupplementLine.Description,
                        Name = quotationSupplementLine.Name,
                        Quantity = quotationSupplementLine.Quantity,
                        SupplementPrice = quotationSupplementLine.SupplementPrice.Value,
                        SupplementVat = quotationSupplementLine.SupplementVat.Value,
                    }),

                })
            }).SingleOrDefaultAsync(x => x.Id == quotationId);

            if (quotation is null)
                throw new EntityNotFoundException(nameof(Quotation), quotationId);

            return quotation;
        }

    }
}
