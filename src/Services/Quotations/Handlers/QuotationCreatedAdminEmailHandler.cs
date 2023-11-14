using Foodtruck.Shared.Emails;
using MediatR;
using Services.Quotations.Notifications;

namespace Services.Quotations.Handlers;

public class QuotationCreatedAdminEmailHandler : INotificationHandler<QuotationCreatedNotification>
{
    private readonly IEmailService emailService;

    public QuotationCreatedAdminEmailHandler(IEmailService emailService)
    {
        this.emailService = emailService;
    }

    public async Task Handle(QuotationCreatedNotification notification, CancellationToken cancellationToken)
    {
        await emailService.SendNewQuotationPdfToAdmin(notification.quotation);
        await Task.CompletedTask;
    }
}
