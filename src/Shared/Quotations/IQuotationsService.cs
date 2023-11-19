using Domain.Quotations;

namespace Foodtruck.Shared.Quotations;

public interface IQuotationService
{
    Task<QuotationResult.Index> GetIndexAsync(QuotationRequest.Index request);
    Task<int> CreateAsync(QuotationDto.Create model);
    Task<QuotationDto.Detail> GetDetailAsync(int quotationId);
    Task<IEnumerable<QuotationDto.Detail>?> GetPreviousVersionsAsync(int quotationId);
}