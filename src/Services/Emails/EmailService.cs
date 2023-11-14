using Domain.Quotations;
using Foodtruck.Shared.Emails;
using Foodtruck.Shared.Pdfs;
using Foodtruck.Shared.Quotations;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Services.Emails;

public class EmailService : IEmailService
{
    private readonly ISendGridClient sendGridClient;
    private readonly IPdfService pdfService;

    private readonly EmailAddress ADMIN_EMAIL = new EmailAddress("blanche.willem@gmail.com", "Willem van Blanche");// When changing this : Sendgrid settings - Sender Authentication - Single Sender Vertification

    public EmailService(ISendGridClient sendGridClient, IPdfService pdfService)
    {
        this.sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
        this.pdfService = pdfService;
    }

    public async Task<bool> SendNewQuotationConfirmationToCustomer(QuotationDto.Detail quotation)
    {
        SendGridMessage msg = new SendGridMessage();
        List<EmailAddress> recipients = new List<EmailAddress> { new EmailAddress(quotation.Customer!.Email, quotation.Customer.Firstname + " " + quotation.Customer.Lastname)};
        msg.SetFrom(ADMIN_EMAIL);
        msg.AddTos(recipients);
        msg.SetSubject("Blanche Offerte Aanvraag");
        msg.PlainTextContent = $"Beste {quotation.Customer.Firstname}, \n\nWe hebben jouw aanvraag goed ontvangen en proberen je zo snel mogelijk een antwoord te bezorgen.\n\nMet vriendelijke groet,\n\n{ADMIN_EMAIL.Name}";
        msg.ReplyTo = ADMIN_EMAIL;
        Response response = await sendGridClient.SendEmailAsync(msg);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SendNewQuotationPdfToAdmin(QuotationDto.Detail quotation)
    {
        string pdfToSendAsBase64 = await pdfService.GetQuotationPdfAsBase64(quotation, quotation.QuotationVersions!.Last());
        string customerFullName = quotation.Customer!.Firstname + " " + quotation.Customer.Lastname;
       
        string filename = $"Offerte_{quotation.Id}_{quotation.Customer.Lastname}_{quotation.Customer.Firstname}";
        string subject = $"Nieuwe offerte aanvraag: {filename}";

        SendGridMessage msg = new SendGridMessage();
        List<EmailAddress> recipients = new List<EmailAddress> { ADMIN_EMAIL };
        msg.SetSubject(subject);
        msg.SetFrom(ADMIN_EMAIL);
        msg.AddTos(recipients);
        msg.PlainTextContent = "Zie nieuwe offerte in bijlage";
        msg.Attachments = new List<Attachment>
        {
            new Attachment
            {
                Content = pdfToSendAsBase64,
                Filename = $"{filename}.pdf",
                Type = "application/pdf",
                Disposition = "attachment"
            }
        };
        msg.ReplyTo = new EmailAddress(quotation.Customer.Email, customerFullName);
        Response response = await sendGridClient.SendEmailAsync(msg);
        return response.IsSuccessStatusCode;
    }
}
