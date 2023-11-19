using Foodtruck.Shared.Customers;

namespace Foodtruck.Shared.Quotations;

public abstract class QuotationResult
{
    public class Index
    {
        public IEnumerable<QuotationDto.Index>? Quotations { get; set; }
        public int TotalAmount { get; set; }
    }

}