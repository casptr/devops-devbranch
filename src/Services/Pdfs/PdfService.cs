using Foodtruck.Shared.Pdfs;
using Foodtruck.Shared.Quotations;
using QuestPDF.Fluent;
using Services.Pdfs.QuotationPdfs;

namespace Services.Pdfs;

public class PdfService : IPdfService
{
    
    public Task<string> GetQuotationPdfAsBase64(QuotationDto.Detail quotation)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        QuotationModel model = new QuotationModel(quotation);
        var document = new QuotationDocument(model);
        string Base64String;
        using (var memstream = new MemoryStream())
        {
            document.GeneratePdf(memstream);
            Base64String = Convert.ToBase64String(memstream.ToArray());
        }
        return Task.FromResult(Base64String);
    }
}
