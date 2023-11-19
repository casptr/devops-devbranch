using Ardalis.GuardClauses;
using Domain.Common;
using Domain.Formulas;
using Domain.Supplements;
namespace Domain.Quotations;


public class QuotationSupplementLine : Entity
{
    public bool IsIncludedInFormula {  get; set; }

    public string Name { get; } = default!;
    public string Description { get; } = default!;

    public Money SupplementPrice { get; } = default!;
    public Money SupplementVat { get; } = default!;

    public int Quantity { get; set; } = default!;
    public int QuotationId { get; set; }

    /// <summary>
    /// Database Constructor
    /// </summary>
    private QuotationSupplementLine() { }

    public QuotationSupplementLine(SupplementItem supplementItem, bool isIncludedInFormula)
    {
        IsIncludedInFormula = isIncludedInFormula;
        Guard.Against.Null(supplementItem, nameof(supplementItem));
        Supplement supplement = supplementItem.Supplement;
        Name = supplement.Name;
        Description = supplement.Description;
        SupplementPrice = supplement.Price.Copy();
        SupplementVat = new Money(supplement.Price.Value * new decimal(supplement.Category.Vat) / 100M);
        Quantity = supplementItem.Quantity;
    }
}
