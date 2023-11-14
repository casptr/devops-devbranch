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
    public FormulaDto.Detail? Formula { get; set; }
    public AddressDto? EventAddress { get; set; }
    public AddressDto? BillingAddress { get; set; }

    public decimal FoodtruckPrice { get; set; }
    public decimal Price { get; set; }
    public decimal VatTotal { get; set; }

    public IEnumerable<QuotationSupplementLineDto>? FormulaSupplementLines { get; set; }
    public IEnumerable<QuotationSupplementLineDto>? ExtraSupplementLines { get; set; }




    public QuotationModel(QuotationDto.Detail quotationDto, QuotationVersionDto.Detail quotationVersion)
    {
        Customer = quotationDto.Customer;
        Id = quotationVersion.Id;
        // Quotation details
        VersionNumber = quotationVersion.VersionNumber;
        NumberOfGuests = quotationVersion.NumberOfGuests;
        ExtraInfo = quotationVersion.ExtraInfo;
        Description = quotationVersion.Description;
        Reservation = quotationVersion.Reservation;
        Formula = quotationVersion.Formula;

        // Address details
        EventAddress = quotationVersion.EventAddress;
        BillingAddress = quotationVersion.BillingAddress;

        // Pricing details
        FoodtruckPrice = quotationVersion.FoodtruckPrice;
        Price = quotationVersion.Price;
        VatTotal = quotationVersion.VatTotal;

        // Supplement lines
        FormulaSupplementLines = quotationVersion.FormulaSupplementLines;
        ExtraSupplementLines = quotationVersion.ExtraSupplementLines;
    }
}
