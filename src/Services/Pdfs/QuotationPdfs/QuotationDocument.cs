using Domain.Common;
using Foodtruck.Shared.Customers;
using Foodtruck.Shared.Quotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Elements;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Services.Pdfs.Helpers;

namespace Services.Pdfs.QuotationPdfs;
public class QuotationDocument : IDocument
{
    QuotationModel Model { get; }

    public QuotationDocument(QuotationModel model)
    {
        Model = model;
    }

    Random randomNumber = new Random();




    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Width(200).Height(50).Placeholder();
                column
                    .Item().Text($"Offerte #10{randomNumber.NextInt64(25, 99)}")
                    .FontSize(14).SemiBold().FontColor(Colors.Grey.Darken3);

                column.Item().Text(text =>
                {
                    text.Span("Opgemaakt op: ").SemiBold();
                    text.Span($"{DateTime.Now:dd/MM/yyyy}");
                });

                column.Item().Text(text =>
                {
                    text.Span("Geldig tot: ").SemiBold();
                    text.Span($"{DateTime.Now.AddDays(30):dd/MM/yyyy}");
                });
            });
            row.RelativeItem().Column(column =>
            {
                column.Item().AlignRight().Text("BLANCHE Mobiele Bar");
                column.Item().AlignRight().Text("Willem Dewaele");
                column.Item().AlignRight().Text("Albert Liénartstraat 19");
                column.Item().AlignRight().Text("9300 Aalst");
                column.Item().AlignRight().Text("BTW: 0825.292.925");
            });
            //row.ConstantItem(175).Image(LogoImage);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(20);

            column.Item().Row(row =>
            {
                row.RelativeItem().Component(new CustomerComponent(Model.Customer));
                row.ConstantItem(50);
                row.RelativeItem().Column(column =>
                {
                    column.Item().Component(new AddressComponent("Factuuradres", Model.BillingAddress));
                    column.Item().Component(new AddressComponent("Evenementadres", Model.EventAddress));
                });
            });

            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Darken1).Element(ComposeQuotationLinesTable);

            //column.Item().BorderBottom(1).BorderColor(Colors.Grey.Darken1).Element(ComposeExtraSupplementsTable);


            if (!string.IsNullOrWhiteSpace(Model.ExtraInfo))
                column.Item().PaddingTop(25).Element(ComposeComments);
        });
    }



    void ComposeQuotationLinesTable(IContainer container)
    {
        container.Table(table =>
        {
            Money formulaTotalPrice = QuotationCalculator.CalculateFormulaPrice(Model);
            Money formulaTotalVat = QuotationCalculator.CalculateFormulaVat(formulaTotalPrice.Value);

            Money extraSupplementsTotalPrice = QuotationCalculator.CalculateExtraSupplementsTotalPrice(Model.ExtraSupplementLines);
            Money extraSupplementsTotalVat = QuotationCalculator.CalculateExtraSupplementsTotalVat(Model.ExtraSupplementLines);

            Money QuotationTotalPrice = new Money(formulaTotalPrice.Value + extraSupplementsTotalPrice.Value);
            Money QuotationTotalVat = new Money(formulaTotalVat.Value + extraSupplementsTotalVat.Value);


            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(2);
                columns.RelativeColumn(6);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
                columns.RelativeColumn(3);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).AlignCenter().Text("Aantal");
                header.Cell().Element(CellStyle).AlignCenter().Text("Omschrijving");
                header.Cell().Element(CellStyle).AlignCenter().Text("Eenhprijs (€)");
                header.Cell().Element(CellStyle).AlignCenter().Text("Bedrag (€)");
                header.Cell().Element(CellStyle).AlignCenter().Text("BTW (€)");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold().LineHeight((float)1.8)).PaddingVertical(0).Border(1).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten1);
                }
            });

            table.Cell().Element(CellStyle).AlignCenter().Text("1");
            table.Cell().Element(CellStyle).Text($"Formule: {Model.Formula.Title}").SemiBold();
            table.Cell().Element(CellStyle).AlignRight().Text($"{formulaTotalPrice}");
            table.Cell().Element(CellStyle).AlignRight().Text($"{formulaTotalPrice}");
            table.Cell().Element(CellStyle).AlignRight().Text($"€{formulaTotalVat}");

            table.Cell().Element(CellStyle).AlignCenter().Text("");
            table.Cell().Element(CellStyle).Text($"Mobiele bar Blanche btw 21%");
            table.Cell().Element(CellStyle).AlignRight().Text($"");
            table.Cell().Element(CellStyle).AlignRight().Text($"");
            table.Cell().Element(CellStyle).AlignRight().Text($"");


            ComposeSupplementLines(table, Model.FormulaSupplementLines, false);
            TableRowSpacer(table);
            ComposeSupplementLines(table, Model.ExtraSupplementLines, true);

            ComposeQuotationTotals(table, QuotationTotalPrice, QuotationTotalVat);

            static IContainer CellStyle(IContainer container)
            {
                return container.BorderVertical(1).BorderColor(Colors.Grey.Medium).PaddingHorizontal(5);
            }
        });
    }

    private void ComposeSupplementLines(TableDescriptor table, IEnumerable<QuotationSupplementLineDto> supplementLines, bool showPrices)
    {
        foreach (var item in supplementLines)
        {
            int supplementVatPercentage = QuotationCalculator.CalculateSupplementVatPercentage(item);
            Money supplementLineTotalPrice = QuotationCalculator.CalculateSupplementLineTotalPrice(item);
            Money supplementLineTotalVat = QuotationCalculator.CalculateSupplementLineTotalVat(supplementLineTotalPrice, supplementVatPercentage);
            
            table.Cell().Element(CellStyle).AlignCenter().Text($"{(showPrices ? item.Quantity : "")}");
            table.Cell().Element(CellStyle).AlignLeft().Text($"{item.Name} btw {supplementVatPercentage}%");
            table.Cell().Element(CellStyle).AlignRight().Text($"{ (showPrices ? new Money(item.SupplementPrice) : "")}");
            table.Cell().Element(CellStyle).AlignRight().Text($"{(showPrices ? supplementLineTotalPrice : "")}");
            table.Cell().Element(CellStyle).AlignRight().Text($"{(showPrices ? supplementLineTotalVat : "")}");
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderVertical(1).BorderColor(Colors.Grey.Medium).PaddingHorizontal(5);
        }
    }

    private void ComposeQuotationTotals(TableDescriptor table, Money QuotationTotalPrice, Money QuotationTotalVat)
    {
        table.Cell().Element(FooterHeader).AlignCenter().Text("Belastbaar");
        table.Cell().ColumnSpan(2).Element(FooterHeader).AlignCenter().Text("BTW");
        table.Cell().ColumnSpan(2).Element(FooterHeader).AlignCenter().Text("Totaal");

        table.Cell().Element(CellStyle).AlignCenter().PaddingTop(1).PaddingBottom(1).Text($"{QuotationTotalPrice}");
        table.Cell().ColumnSpan(2).Element(CellStyle).AlignCenter().PaddingTop(1).PaddingBottom(1).Text($"{QuotationTotalVat}");
        table.Cell().ColumnSpan(2).Element(CellStyle).AlignCenter().PaddingTop(1).PaddingBottom(1).Text($"{new Money(QuotationTotalPrice.Value + QuotationTotalVat.Value)}");

        static IContainer FooterHeader(IContainer container)
        {
            return container.DefaultTextStyle(x => x.FontSize(11).LineHeight((float)1.8)).PaddingVertical(0).Border(1).BorderBottom(0).BorderColor(Colors.Grey.Darken1).Background(Colors.Grey.Lighten2);
        }

        static IContainer CellStyle(IContainer container)
        {
            return container.BorderVertical(1).BorderColor(Colors.Grey.Medium).PaddingHorizontal(5);
        }
    }

    private void TableRowSpacer(TableDescriptor table)
    {
        table.Cell().Element(CellStyle).AlignCenter().Text("");
        table.Cell().Element(CellStyle).AlignLeft().Text("");
        table.Cell().Element(CellStyle).AlignRight().Text("");
        table.Cell().Element(CellStyle).AlignRight().Text("");
        table.Cell().Element(CellStyle).AlignRight().Text("");
        static IContainer CellStyle(IContainer container)
        {
            return container.BorderVertical(1).BorderColor(Colors.Grey.Medium).PaddingHorizontal(5);
        }
    }


    void ComposeComments(IContainer container)
    {
        container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Column(column =>
        {
            column.Spacing(5);
            column.Item().Text("Extra informatie").FontSize(14).SemiBold();
            column.Item().Text(Model.ExtraInfo);
        });
    }
}

public class CustomerComponent : IComponent
{
    public string? Firstname { get; }
    public string? Lastname { get;}
    public string? Email { get; }
    public string? Phone { get; }
    public string? CompanyName { get; }
    public string? CompanyNumber { get; }
    public CustomerComponent(CustomerDto.Detail customer ) 
    { 
        Firstname = customer.Firstname; 
        Lastname = customer.Lastname;
        Email = customer.Email;
        Phone = customer.Phone;
        CompanyName = customer.CompanyName;
        CompanyNumber = customer.CompanyNumber;
    }

    public void Compose(IContainer container) 
    { 
        container.ShowEntire().Column(column =>
        {
            column.Spacing(0);
            column.Item().Text("Aan").SemiBold();
            column.Item().PaddingBottom(2).LineHorizontal(1);
            column.Item().Text($"{Firstname} {Lastname}");
            column.Item().Text($" {Email}");
            column.Item().Text($"{Phone}");
            column.Item().Text($"{CompanyName}");
            column.Item().Text($"{CompanyNumber}");
            
        });
    }
}

public class AddressComponent : IComponent
{
    private string Title { get; }
    private AddressDto Address { get; }

    public AddressComponent(string title, AddressDto address)
    {
        Title = title;
        Address = address;
    }

    public void Compose(IContainer container)
    {
        container.ShowEntire().Column(column =>
        {
            column.Spacing(2);
            column.Item().Text(Title).SemiBold();
            column.Item().PaddingBottom(2).LineHorizontal(1);
            column.Item().Text($"{Address.Street} {Address.HouseNumber}");
            column.Item().Text($"{Address.City}, {Address.Zip}");
        });
    }
}

