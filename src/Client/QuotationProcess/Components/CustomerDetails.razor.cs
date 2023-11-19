using Microsoft.AspNetCore.Components;
using MudBlazor;
using Foodtruck.Client.QuotationProcess.Helpers;
namespace Foodtruck.Client.QuotationProcess.Components
{
    public partial class CustomerDetails
    {
        [CascadingParameter] private QuotationProcessStepControl QuotationProcessStepControl { get; set; }
        [Inject] public QuotationProcessState QuotationProcessState { get; set; } = default!;
        private CustomerDetailsFormModel model => QuotationProcessState.CustomerDetailsFormModel;
        private readonly CustomerDetailsFormModel.Validator validator = new();

        private MudForm form = default!;

        bool useTestData = true;

        // For testing purposes TODO delete this
        protected override void OnInitialized()
        {
            if (QuotationProcessStepControl == null)
                throw new ArgumentNullException(nameof(QuotationProcessStepControl), "CustomerDetails must be used inside a QuotationProcessStep");

            if (!useTestData)
                return;
            model.Customer.WantsMarketingMails = true;
            model.Customer.Firstname = "Bart";
            model.Customer.Lastname = "Vandenbranden";
            model.Customer.CompanyName = "Vandenbranden Corp";
            model.Customer.CompanyNumber = "123.345.278";
            model.Customer.Email = "bvandenbranden@vbcorp.be";
            model.Customer.Phone = "0470454372";
            model.EventAddress.Street = "Sint-Denijslaan";
            model.EventAddress.HouseNumber = "230";
            model.EventAddress.City = "Gent";
            model.EventAddress.Zip = "9000";
            model.BillingAddress.Street = "Berkenlaan";
            model.BillingAddress.HouseNumber = "45";
            model.BillingAddress.City = "Melle";
            model.BillingAddress.Zip = "9090";
            model.IsEventAddressDifferentThanBillingAddress = true;
            model.ExtraInfo = "Het gaat om een bedrijfsfeest. Je mag mij gerust bellen om de offerte te bespreken.";

        }

        public async Task Submit()
        {
            await form.Validate();

            if (!form.IsValid)
                return;

            QuotationProcessState.ConfigureQuotationCustomerDetails();
            QuotationProcessState.PrintQuotation();
            if (QuotationProcessState.ReachedEnd)
                QuotationProcessStepControl.GoToOverview();
            else
                QuotationProcessStepControl.NextStep();
        }
    }
}