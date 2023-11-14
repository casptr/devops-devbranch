using Foodtruck.Shared.Quotations;
using MediatR;

namespace Services.Quotations.Notifications;

public record QuotationCreatedNotification(QuotationDto.Detail quotation) : INotification;
