using Foodtruck.Client.QuotationProcess.Helpers;
using Foodtruck.Shared.Reservations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using FluentValidation;

namespace Foodtruck.Client.QuotationProcess.Components
{
    public partial class Calendar
    {
        [CascadingParameter] private QuotationProcessStepControl QuotationProcessStepControl { get; set; } = default!;
        [Inject] private QuotationProcessState QuotationProcessState { get; set; } = default!;
        [Inject] private IReservationService ReservationService { get; set; } = default!;
        [CascadingParameter] private MudTheme? Theme { get; set; }
        private ReservationDto.Create Model => QuotationProcessState.ReservationModel;
        private readonly ReservationDto.Validator calendarValidator = new();
        private MudForm form = default!;

        private IEnumerable<ReservationDto.Index>? reservations;
        private bool startDateConfirmed = false;
        private bool loading = true;

        protected override async Task OnInitializedAsync()
        {
            bool useTestData = false;
            if (useTestData)
            {
                Model.Start = DateTime.Now.Date.AddDays(1);
                Model.End = DateTime.Now.Date.AddDays(2);
            }

            if (Model.End is not null) startDateConfirmed = true;

            ReservationRequest.Index request = new ReservationRequest.Index();
            reservations = (await ReservationService.GetIndexAsync(request)).Reservations;

            if (reservations is not null)
                foreach (var r in reservations)
                {
                    Console.WriteLine($"{r.Id} - {r.Start} - {r.End}");
                }

            loading = false;
            StateHasChanged();
        }

        private DateTime? PickerMonth => new(Model.Start?.Year ?? DateTime.Now.Year, Model.Start?.Month ?? DateTime.Now.Month, 1);
        private string ChipStyle(bool condition) => "color:" + (condition ? Theme?.Palette.PrimaryContrastText : Theme?.Palette.PrimaryLighten);
        private Variant ChipVariant(bool condition) => condition ? Variant.Filled : Variant.Outlined;

        private void ConfirmStartDate()
        {
            startDateConfirmed = true;
            Model.End = Model.Start;
        }
        private void EditStartDate()
        {
            Model.End = null;
            startDateConfirmed = false;
        }

        protected async Task Submit()
        {
            await form.Validate();

            if (!form.IsValid)
            {
                return;
            }

            QuotationProcessState.ConfigureQuotationReservation();
            QuotationProcessStepControl.NextStep();
        }

        private async void GoToOverview()
        {
            await form.Validate();

            if (!form.IsValid)
            {
                return;
            }

            QuotationProcessState.ConfigureQuotationReservation();
            QuotationProcessStepControl.GoToStep(4, true);
        }

        // MudDatePicker Functions
        private bool IsDateAlreadyBooked(DateTime dateTime) =>
            reservations is not null && reservations.Any(reservation =>
                dateTime.Date >= reservation.Start.Date &&
                dateTime.Date <= reservation.End.Date ||
                dateTime.Date <= DateTime.Now.Date);

        private bool IsDateAvailableAsEnd(DateTime dateTime)
        {
            if (reservations is null || !reservations.Any() || Model.Start is null)
                return true;

            ReservationDto.Index firstReservation = reservations
                .OrderBy(reservation => reservation.Start)
                .Where(reservation => reservation.Start.Date > Model.Start?.Date)
                .First();

            return reservations
                .Any(reservation => dateTime.Date < Model.Start?.Date || dateTime.Date >= firstReservation.Start.Date);
        }

        private string AdditionalDateClassesFunc(DateTime dateTime) =>
            Model.Start is null ? "" :
            dateTime.Date == Model.Start?.Date && Model.Start?.Date == Model.End?.Date ? "mud-selected" :
            dateTime.Date == Model.Start?.Date ? "mud-range mud-range-start-selected mud-theme-primary" :
            dateTime.Date == Model.End?.Date ? "mud-range mud-range-end-selected mud-theme-primary" :
            dateTime.Date > Model.Start?.Date && dateTime.Date < Model.End?.Date ? "mud-range mud-range-between mud-theme-primary" : "";



        protected override void OnInitialized()
        {
            if (QuotationProcessStepControl == null)
                throw new ArgumentNullException(nameof(QuotationProcessStepControl), "Calendar must be used inside a QuotationProcessStep");

            base.OnInitialized();
        }
    }
}
