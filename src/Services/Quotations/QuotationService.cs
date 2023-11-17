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
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.Quotations.Notifications;
using System;

namespace Services.Quotations
{
    public class QuotationService : IQuotationService
    {
        private readonly FoodtruckDbContext dbContext;
        private readonly IMediator mediator;

        public QuotationService(FoodtruckDbContext dbContext, IMediator mediator)
        {
            this.dbContext = dbContext;
            this.mediator = mediator;
        }

        public async Task<QuotationResult.Index> GetIndexAsync(QuotationRequest.Index request)
        {
            var query = dbContext.Quotations.AsQueryable();

            int totalAmount = await query.CountAsync();

            var items = await query
           .Skip((request.Page - 1) * request.PageSize)
           .Take(request.PageSize)
           .OrderBy(x => x.Id)
           .Where(x => x.MostRecentVersion == null)
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

               NumberOfGuests = x.NumberOfGuests,
               ExtraInfo = x.ExtraInfo,
               Description = x.Description,
               Reservation = new ReservationDto.Detail()
               {
                   Id = x.Reservation.Id,
                   Description = x.Reservation.Description,
                   Start = x.Reservation.Start,
                   End = x.Reservation.End,
                   Status = (StatusDto)((int)x.Reservation.Status),
               },
               Formula = new FormulaDto.Index()
               {
                   Id = x.Formula.Id,
                   Title = x.Formula.Title,
               },
               EventAddress = new AddressDto()
               {
                   Street = x.EventAddress.Street,
                   City = x.EventAddress.City,
                   HouseNumber = x.EventAddress.HouseNumber,
                   Zip = x.EventAddress.Zip
               },
               BillingAddress = new AddressDto()
               {
                   Street = x.BillingAddress.Street,
                   City = x.BillingAddress.City,
                   Zip = x.BillingAddress.Zip,
                   HouseNumber = x.BillingAddress.HouseNumber,
               },
               FoodtruckPrice = x.FoodtruckPrice.Value,
               Price = x.Price.Value,
               VatTotal = x.VatTotal.Value,
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

            Formula? formula = await dbContext.Formulas.Include(x => x.Foodtruck.PricePerDays).SingleOrDefaultAsync(x => x.Id == model.FormulaId);

            if (formula is null)
                throw new EntityNotFoundException(nameof(Formula), model.FormulaId);


            IEnumerable<SupplementItemDto.Create> allSupplementItemDtos = model.ExtraSupplementItems.Concat(model.FormulaSupplementItems);

            IEnumerable<Supplement> supplements = await dbContext.Supplements
                                               .Where(x => allSupplementItemDtos.Select(x => x.SupplementId).Contains(x.Id)).Include(x => x.Category)
                                               .ToListAsync();



            List<SupplementItem> formulaSupplementItems = new();
            List<SupplementItem> extraSupplementItems = new();

            foreach (var item in model.FormulaSupplementItems)
            {
                Supplement? s = supplements.FirstOrDefault(x => x.Id == item.SupplementId);
                if (s is null)
                    throw new EntityNotFoundException(nameof(Supplement), item.SupplementId);

                formulaSupplementItems.Add(new SupplementItem(s, item.Quantity));
            }


            foreach (var item in model.ExtraSupplementItems)
            {
                Supplement? s = supplements.FirstOrDefault(x => x.Id == item.SupplementId);
                if (s is null)
                    throw new EntityNotFoundException(nameof(Supplement), item.SupplementId);

                extraSupplementItems.Add(new SupplementItem(s, item.Quantity));
            }


            Address eventAddress = new Address(model.EventAddress.Zip, model.EventAddress.City, model.EventAddress.Street, model.EventAddress.HouseNumber);
            Address billingAddress = new Address(model.BillingAddress.Zip, model.BillingAddress.City, model.BillingAddress.Street, model.BillingAddress.HouseNumber);
            string reservationDescription = model.Customer.Firstname + " " + model.Customer.Lastname;
            Reservation reservation = new Reservation(model.Reservation.Start.Value, model.Reservation.End.Value, reservationDescription);

            Quotation newQuotation = new Quotation(customer, model.NumberOfGuests, model.ExtraInfo, "No description", reservation, formula, formulaSupplementItems, extraSupplementItems, eventAddress, billingAddress);

            dbContext.Quotations.Add(newQuotation);
            await dbContext.SaveChangesAsync();

            // Create notification - TODO: should real quotation be used here? Not DTO
            QuotationDto.Detail quotationDto = await GetDetailAsync(newQuotation.Id);
            await mediator.Publish(new QuotationCreatedNotification(quotationDto));

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
                NumberOfGuests = x.NumberOfGuests,
                ExtraInfo = x.ExtraInfo,
                Description = x.Description,
                Reservation = new ReservationDto.Detail()
                {
                    Id = x.Reservation.Id,
                    Description = x.Reservation.Description,
                    Start = x.Reservation.Start,
                    End = x.Reservation.End,
                    Status = (StatusDto)((int)x.Reservation.Status),
                },
                Formula = new FormulaDto.Index()
                {
                    Id = x.Formula.Id,
                    Title = x.Formula.Title,
                },
                EventAddress = new AddressDto()
                {
                    Street = x.EventAddress.Street,
                    City = x.EventAddress.City,
                    HouseNumber = x.EventAddress.HouseNumber,
                    Zip = x.EventAddress.Zip
                },
                BillingAddress = new AddressDto()
                {
                    Street = x.BillingAddress.Street,
                    City = x.BillingAddress.City,
                    Zip = x.BillingAddress.Zip,
                    HouseNumber = x.BillingAddress.HouseNumber,
                },
                FoodtruckPrice = x.FoodtruckPrice.Value,
                Price = x.Price.Value,
                VatTotal = x.VatTotal.Value,
                FormulaSupplementLines = x.QuotationSupplementLines.Where(quotationSupplementLine => quotationSupplementLine.IsIncludedInFormula).Select(quotationSupplementLine => new QuotationSupplementLineDto()
                {
                    Id = quotationSupplementLine.Id,
                    Description = quotationSupplementLine.Description,
                    Name = quotationSupplementLine.Name,
                    Quantity = quotationSupplementLine.Quantity,
                    SupplementPrice = quotationSupplementLine.SupplementPrice.Value,
                    SupplementVat = quotationSupplementLine.SupplementVat.Value,
                }),

                ExtraSupplementLines = x.QuotationSupplementLines.Where(quotationSupplementLine => !quotationSupplementLine.IsIncludedInFormula).Select(quotationSupplementLine => new QuotationSupplementLineDto()
                {
                    Id = quotationSupplementLine.Id,
                    Description = quotationSupplementLine.Description,
                    Name = quotationSupplementLine.Name,
                    Quantity = quotationSupplementLine.Quantity,
                    SupplementPrice = quotationSupplementLine.SupplementPrice.Value,
                    SupplementVat = quotationSupplementLine.SupplementVat.Value,
                }),

            }).SingleOrDefaultAsync(x => x.Id == quotationId);

            if (quotation is null)
                throw new EntityNotFoundException(nameof(Quotation), quotationId);

            return quotation;
        }


        public async Task<IEnumerable<QuotationDto.Detail>?> GetPreviousVersionsAsync(int quotationId)
        {
            List<QuotationDto.Detail> previousVersions = await dbContext.Quotations.Where(x => x.MostRecentVersion != null && x.MostRecentVersion.Id == quotationId).Select(x => new QuotationDto.Detail()
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
                NumberOfGuests = x.NumberOfGuests,
                ExtraInfo = x.ExtraInfo,
                Description = x.Description,
                Reservation = new ReservationDto.Detail()
                {
                    Id = x.Reservation.Id,
                    Description = x.Reservation.Description,
                    Start = x.Reservation.Start,
                    End = x.Reservation.End,
                    Status = (StatusDto)((int)x.Reservation.Status),
                },
                Formula = new FormulaDto.Index()
                {
                    Id = x.Formula.Id,
                    Title = x.Formula.Title,
                },
                EventAddress = new AddressDto()
                {
                    Street = x.EventAddress.Street,
                    City = x.EventAddress.City,
                    HouseNumber = x.EventAddress.HouseNumber,
                    Zip = x.EventAddress.Zip
                },
                BillingAddress = new AddressDto()
                {
                    Street = x.BillingAddress.Street,
                    City = x.BillingAddress.City,
                    Zip = x.BillingAddress.Zip,
                    HouseNumber = x.BillingAddress.HouseNumber,
                },
                FoodtruckPrice = x.FoodtruckPrice.Value,
                Price = x.Price.Value,
                VatTotal = x.VatTotal.Value,
                FormulaSupplementLines = x.QuotationSupplementLines.Where(quotationSupplementLine => quotationSupplementLine.IsIncludedInFormula).Select(quotationSupplementLine => new QuotationSupplementLineDto()
                {
                    Id = quotationSupplementLine.Id,
                    Description = quotationSupplementLine.Description,
                    Name = quotationSupplementLine.Name,
                    Quantity = quotationSupplementLine.Quantity,
                    SupplementPrice = quotationSupplementLine.SupplementPrice.Value,
                    SupplementVat = quotationSupplementLine.SupplementVat.Value,
                }),

                ExtraSupplementLines = x.QuotationSupplementLines.Where(quotationSupplementLine => !quotationSupplementLine.IsIncludedInFormula).Select(quotationSupplementLine => new QuotationSupplementLineDto()
                {
                    Id = quotationSupplementLine.Id,
                    Description = quotationSupplementLine.Description,
                    Name = quotationSupplementLine.Name,
                    Quantity = quotationSupplementLine.Quantity,
                    SupplementPrice = quotationSupplementLine.SupplementPrice.Value,
                    SupplementVat = quotationSupplementLine.SupplementVat.Value,
                }),

            }).ToListAsync();


            return previousVersions;
        }




    }
}
