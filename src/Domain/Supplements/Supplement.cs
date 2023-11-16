using Ardalis.GuardClauses;
using Domain.Common;
using Domain.Formulas;

namespace Domain.Supplements;

public class Supplement : Entity
{
    private string name = default!;
    public string Name
    {
        get => name;
        set => name = Guard.Against.NullOrWhiteSpace(value, nameof(Name));
    }

    private string description = default!;
    public string Description
    {
        get => description;
        set => description = Guard.Against.NullOrWhiteSpace(value, nameof(Description));
    }

    private Category category = default!;
    public Category Category
    {
        get => category;
        set => category = Guard.Against.Null(value, nameof(Category));
    }

    private Money price = default!;
    public Money Price
    {
        get => price;
        set => price = Guard.Against.Null(value, nameof(Price));
    }

    private readonly List<SupplementImage> imageUrls = new();
    public IReadOnlyCollection<SupplementImage> ImageUrls => imageUrls.AsReadOnly();

    private int amountAvailable = default!;
    public int AmountAvailable
    {
        get => amountAvailable;
        set => amountAvailable = Guard.Against.Negative(value, nameof(AmountAvailable));
    }

    private readonly List<FormulaSupplementChoice> formulaSupplementChoices = new();
    public IReadOnlyCollection<FormulaSupplementChoice> FormulaSupplementChoices => formulaSupplementChoices.AsReadOnly();

    private readonly List<FormulaSupplementChoice> formulaSupplementDefaultChoices = new();
    public IReadOnlyCollection<FormulaSupplementChoice> FormulaSupplementDefaultChoices => formulaSupplementDefaultChoices.AsReadOnly();

    private Supplement() { }

    public Supplement(string name, string description, Category category, Money price, int amountAvailable)
    {
        Name = name;
        Description = description;
        Category = category;
        Price = price;
        AmountAvailable = amountAvailable;
    }

    public void AddImageUrl(Uri imageUrl)
    {
        Guard.Against.Null(imageUrl, nameof(imageUrl));

        if (imageUrls.Any(x => x.Image == imageUrl))
            throw new ApplicationException($"{nameof(Supplement)} '{name}' already contains the imageUrl:{imageUrl}");
        
        imageUrls.Add(new SupplementImage(imageUrl, this));
    }

    public SupplementImage? GetSupplementImage(Uri imageUrl)
    {
        Guard.Against.Null(imageUrl, nameof(imageUrl));

        if (!imageUrls.Any(x => x.Image == imageUrl))
            throw new ApplicationException($"{nameof(Supplement)} '{name}' does not contain the imageUrl:{imageUrl}");
        
        return imageUrls.Where(url => url.Image == imageUrl).FirstOrDefault();
    }

}