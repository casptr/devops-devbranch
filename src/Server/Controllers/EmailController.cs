using Foodtruck.Shared.Emails;
using Foodtruck.Shared.Pdfs;
using Foodtruck.Shared.Quotations;
using Microsoft.AspNetCore.Mvc;
using Services.Emails;
using Services.Pdfs;
using Swashbuckle.AspNetCore.Annotations;

namespace Foodtruck.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailController : Controller
{
    private readonly IQuotationService quotationService;
    private readonly IEmailService emailService;
    private readonly IPdfService pdfService;
    public EmailController(IQuotationService quotationService, IEmailService emailService, IPdfService pdfService)
    {
        this.quotationService = quotationService;
        this.emailService = emailService;
        this.pdfService = pdfService;
    }

    [SwaggerOperation("Sends a quotation pdf via mail")]
    [HttpGet("{quotationId}")]
    public async Task<bool> SendQuotationPdfEmail(int quotationId)
    {
        Console.WriteLine("Talking to api mail/id");
        QuotationDto.Detail quotation = await quotationService.GetDetailAsync(quotationId);
        if (quotation != null)
        {
            await emailService.SendNewQuotationPdfToAdmin(quotation);
        }
        return true;
    }
}

