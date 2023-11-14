using Foodtruck.Shared.Emails;
using MediatR;
using Services.Quotations.Notifications;

namespace Services.Quotations.Handlers;

public class QuotationCreatedCustomerEmailHandler : INotificationHandler<QuotationCreatedNotification>
{
    private readonly IEmailService emailService;

    public QuotationCreatedCustomerEmailHandler(IEmailService emailService)
    {
        this.emailService = emailService;
    }

    public async Task Handle(QuotationCreatedNotification notification, CancellationToken cancellationToken)
    {
        await emailService.SendNewQuotationConfirmationToCustomer(notification.quotation);
        await Task.CompletedTask;
    }
}
