using Domain.Quotations;
using Foodtruck.Shared.Customers;
using Foodtruck.Shared.Formulas;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Reservations;
using Foodtruck.Shared.Supplements;

namespace Foodtruck.Client.QuotationProcess.Helpers
{
    public class QuotationProcessState
    {
        public bool ReachedEnd { get; set; } = false;
        public int? CurrentStepIndex { get; set; }

        public QuotationDto.Create Quotation { get; } = new();
        public CustomerDto.Create Customer => Quotation.Customer;

        // Reservation
        public ReservationDto.Create ReservationModel { get; set; } = new();

        // Formula
        public FormulaDto.Detail? CurrentSelectedFormula { get; private set; }

        private readonly List<FormulaChoiceFormModel> completedChoiceFormModels = new();
        private readonly List<FormulaChoiceFormModel> choiceFormModels = new();
        public IReadOnlyList<FormulaChoiceFormModel> ChoiceFormModels => choiceFormModels.AsReadOnly();
        public IReadOnlyCollection<FormulaChoiceFormItem> ChosenFormulaChoiceItems => // Item is chosen if quantity != 0
            completedChoiceFormModels.SelectMany(choice => choice.Options.Where(option => option.Quantity != 0)).ToList();

        // Supplements
        private readonly List<ExtraSupplementLine> extraSupplementLines = new();
        public IReadOnlyCollection<ExtraSupplementLine> ExtraSupplementLines => extraSupplementLines.AsReadOnly();

        // CustomerDetails
        public CustomerDetailsFormModel CustomerDetailsFormModel { get; set; } = new();

        public void ConfigureQuotationReservation()
        {
            QuotationVersion.Reservation.Start = ReservationModel.Start?.Date.AddHours(11);
            QuotationVersion.Reservation.End = ReservationModel.End?.Date.AddHours(16);
        }

        public void ConfigureQuotationFormula(FormulaDto.Detail formula)
        {
            completedChoiceFormModels.Clear();
            completedChoiceFormModels.AddRange(choiceFormModels.ToList());
            Quotation.FormulaSupplementItems.Clear();

            CurrentSelectedFormula = formula;
            Quotation.FormulaId = CurrentSelectedFormula.Id;

            Quotation.FormulaSupplementItems.AddRange(completedChoiceFormModels
                .SelectMany(choiceForm => choiceForm.Options)
                .Where(option => option.Quantity != 0)
                .Select(option => new SupplementItemDto.Create()
                {
                    SupplementId = option.Supplement.Id,
                    Quantity = option.Quantity
                }));

            // Add included supplements from formula
            if (CurrentSelectedFormula.IncludedSupplements != null)
                Quotation.FormulaSupplementItems.AddRange(CurrentSelectedFormula.IncludedSupplements
                    .Select(includedSupplementLine => new SupplementItemDto.Create()
                    {
                        SupplementId = includedSupplementLine.Supplement!.Id,
                        Quantity = includedSupplementLine.Quantity
                    }));
        }

        public void ConfigureQuotationExtraSupplements()
        {
            Quotation.ExtraSupplementItems.Clear();
            Quotation.ExtraSupplementItems.AddRange(extraSupplementLines.Select(extraSupplementLine => new SupplementItemDto.Create()
            {
                Quantity = extraSupplementLine.Quantity,
                SupplementId = extraSupplementLine.Supplement.Id,
            }));
        }

        public void ConfigureQuotationCustomerDetails()
        {
            Quotation.NumberOfGuests = 50; // TODO this should be a form field !!!!!!!!!

            Quotation.Customer = CustomerDetailsFormModel.Customer;
            Quotation.EventAddress = CustomerDetailsFormModel.EventAddress;
            Quotation.ExtraInfo = CustomerDetailsFormModel.ExtraInfo;

            // we have to set the billing address if event address is the same as the billing address
            if (CustomerDetailsFormModel.IsEventAddressDifferentThanBillingAddress)
                Quotation.BillingAddress = CustomerDetailsFormModel.BillingAddress;
            else
                Quotation.BillingAddress = CustomerDetailsFormModel.EventAddress;
        }

        public void SetupFormulaChoiceFormModels(FormulaDto.Detail formula)
        {
            choiceFormModels.Clear();

            if (formula.Choices == null || !formula.Choices.Any())
                return;

            choiceFormModels.AddRange(formula.Choices.Select(choice => new FormulaChoiceFormModel(choice)));

            // Apply the previously chosen quantities
            if (CurrentSelectedFormula?.Id == formula.Id)
                for (int choiceIndex = 0; choiceIndex < choiceFormModels.Count; choiceIndex++)
                    for (int optionIndex = 0; optionIndex < choiceFormModels[choiceIndex].Options.Count(); optionIndex++)
                        choiceFormModels[choiceIndex].Options[optionIndex].Quantity = completedChoiceFormModels[choiceIndex].Options[optionIndex].Quantity;
        }

        public int CalculateMaxAmountToAdd(ExtraSupplementLine extraSupplementLine)
        {
            ExtraSupplementLine? extraSupplementLineInState = extraSupplementLines.Where(e => e.Equals(extraSupplementLine)).FirstOrDefault();

            if (extraSupplementLineInState is null)
                return extraSupplementLine.Supplement.AmountAvailable;

            int currentAmount = extraSupplementLineInState.Quantity;
            return extraSupplementLineInState.Supplement.AmountAvailable - currentAmount;
        }

        public void RemoveExtraSupplementLine(ExtraSupplementLine extraSupplementLine)
        {
            extraSupplementLines.Remove(extraSupplementLine);
        }

        public void AddExtraSupplementLine(ExtraSupplementLine extraSupplementLineToAdd)
        {
            // TODO: Add check to see if quantity that is gonna be added doesn't go over the AmountAvailable otherwise user can add more than available amount of a certain item
            ExtraSupplementLine? extraSupplementLineInState = extraSupplementLines.Where(extraSupplementLine => extraSupplementLine.Equals(extraSupplementLineToAdd)).FirstOrDefault();

            if (extraSupplementLineInState is null)
                extraSupplementLines.Add(extraSupplementLineToAdd);
            else
                extraSupplementLineInState.Quantity += extraSupplementLineToAdd.Quantity;
        }

        // Finish quotation request
        public void RequestQuotation()
        {
            // TEMP
            PrintQuotation();
        }
        

        public void PrintQuotation()
        {
            AddressDto EventAddress = Quotation.EventAddress;
            AddressDto BillingAddress = Quotation.BillingAddress;

            Console.WriteLine("------------QUOTATION----------------");
            Console.WriteLine($"Reservation from {Quotation.Reservation.Start} to {Quotation.Reservation.End}");
            Console.WriteLine();

            Console.WriteLine("Customer Details:");
            Console.WriteLine($"Firstname: {Customer.Firstname}");
            Console.WriteLine($"Lastname: {Customer.Lastname}");
            Console.WriteLine($"Email: {Customer.Email}");
            Console.WriteLine($"Phone: {Customer.Phone}");
            Console.WriteLine($"CompanyName: {Customer.CompanyName}");
            Console.WriteLine($"CompanyNumber: {Customer.CompanyNumber}");
            Console.WriteLine($"WantsMarketingMails: {Customer.WantsMarketingMails}");

            Console.WriteLine();
            Console.WriteLine("Event adress:");
            Console.WriteLine($"Street: {EventAddress.Street}");
            Console.WriteLine($"Housenumber: {EventAddress.HouseNumber}");
            Console.WriteLine($"City: {EventAddress.City}");
            Console.WriteLine($"Zip: {EventAddress.Zip}");

            Console.WriteLine();
            Console.WriteLine("Billing adress:");
            Console.WriteLine($"Street: {BillingAddress.Street}");
            Console.WriteLine($"Housenumber: {BillingAddress.HouseNumber}");
            Console.WriteLine($"City: {BillingAddress.City}");
            Console.WriteLine($"Zip: {BillingAddress.Zip}");

            // Temp to have supplements name in writeline here
            List<SupplementDto.Detail> allSupplements = new List<SupplementDto.Detail>();
            allSupplements.AddRange(choiceFormModels.SelectMany(choice => choice.Options.Select(option => option.Supplement)).ToHashSet());
            allSupplements.AddRange(CurrentSelectedFormula.IncludedSupplements.Select(i => i.Supplement));

            Console.WriteLine();
            Console.WriteLine("Supplements included and chosen:");
            foreach (var supplementItem in Quotation.FormulaSupplementItems)
            {
                var supplement = allSupplements.Find(s => s.Id == supplementItem.SupplementId);
                Console.WriteLine($"Formula Supplement: {supplement.Name}, Quantity: {supplementItem.Quantity}");
            }

            Console.WriteLine("---------------------------------------");


        }



    }

}