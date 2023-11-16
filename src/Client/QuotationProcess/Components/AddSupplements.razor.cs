using global::Microsoft.AspNetCore.Components;
using Foodtruck.Shared.Supplements;
using Foodtruck.Client.QuotationProcess.Helpers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Linq;

namespace Foodtruck.Client.QuotationProcess.Components;

public partial class AddSupplements
{
    [CascadingParameter] private QuotationProcessStepControl QuotationProcessStepControl { get; set; }
    [Inject] public QuotationProcessState QuotationProcessState { get; set; } = default!;
    [Inject] public ISupplementService SupplementService { get; set; } = default!;

    private IEnumerable<ExtraSupplementLine>? supplements;

    bool open = true;
    private int count = 0;

    void ToggleStartDrawer()
    {
        open = !open;
        count++;
    }

    protected override async Task OnParametersSetAsync()
    {
        SupplementRequest.Index request = new SupplementRequest.Index()
        {
            Category = "Extra"
        };
        var response = await SupplementService.GetIndexAsync(request);

        supplements = response.Supplements?.Select(supplement =>
        {
            ExtraSupplementLine? extraSupplement = QuotationProcessState.ExtraSupplementLines.Where(supplementLineInState => supplementLineInState.Equals(supplement)).FirstOrDefault();
            return new ExtraSupplementLine()
            {
                Supplement = supplement,
                Quantity = extraSupplement is null ? 0 : extraSupplement.Quantity,
            };
        }).ToList();
    }

    private void AddSupplement(SupplementDto.Detail supplement, int quantity)
    {
        if (quantity == 0)
            return;


        var supplementChoice = new ExtraSupplementLine()
        {
            Supplement = supplement,
            Quantity = quantity
        };

        QuotationProcessState.AddExtraSupplementLine(supplementChoice);
    }

    private void Submit()
    {
        QuotationProcessState.ConfigureQuotationExtraSupplements();
        QuotationProcessStepControl.NextStep();
    }

    private void GoToOverview()
    {
		QuotationProcessState.ConfigureQuotationExtraSupplements();
        QuotationProcessStepControl.GoToStep(4, true);
	}

	protected override void OnInitialized()
    {
        if (QuotationProcessStepControl == null)
            throw new ArgumentNullException(nameof(QuotationProcessStepControl), "AddSupplements must be used inside a QuotationProcessStep");

        base.OnInitialized();
    }
}