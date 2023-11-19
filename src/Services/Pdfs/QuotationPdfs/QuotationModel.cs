using Foodtruck.Shared.Customers;
using Foodtruck.Shared.Formulas;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Reservations;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Pdfs.QuotationPdfs;

public class QuotationModel
{
    public CustomerDto.Detail Customer { get; set; }
    public int Id { get; set; }
    public int VersionNumber { get; set; }
    public int NumberOfGuests { get; set; }
    public string? ExtraInfo { get; set; }
    public string? Description { get; set; }
    public ReservationDto.Detail? Reservation { get; set; }
    public FormulaDto.Index? Formula { get; set; }
    public AddressDto? EventAddress { get; set; }
    public AddressDto? BillingAddress { get; set; }

    public decimal FoodtruckPrice { get; set; }
    public decimal Price { get; set; }
    public decimal VatTotal { get; set; }

    public IEnumerable<QuotationSupplementLineDto>? FormulaSupplementLines { get; set; }
    public IEnumerable<QuotationSupplementLineDto>? ExtraSupplementLines { get; set; }




    public QuotationModel(QuotationDto.Detail quotation)
    {
        Customer = quotation.Customer;
        Id = quotation.Id;
        // Quotation details
        VersionNumber = quotation.VersionNumber;
        NumberOfGuests = quotation.NumberOfGuests;
        ExtraInfo = quotation.ExtraInfo;
        Description = quotation.Description;
        Reservation = quotation.Reservation;
        Formula = quotation.Formula;

        // Address details
        EventAddress = quotation.EventAddress;
        BillingAddress = quotation.BillingAddress;

        // Pricing details
        FoodtruckPrice = quotation.FoodtruckPrice;
        Price = quotation.Price;
        VatTotal = quotation.VatTotal;

        // Supplement lines
        FormulaSupplementLines = quotation.FormulaSupplementLines;
        ExtraSupplementLines = quotation.ExtraSupplementLines;
    }
}
