using Domain.Common;
using Foodtruck.Shared.Customers;
using Foodtruck.Shared.Formulas;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Reservations;
using Foodtruck.Shared.Supplements;
using Services.Pdfs.Helpers;
using Services.Pdfs.QuotationPdfs;
using Shouldly;

namespace Services.Tests.Pdfs.Helpers;
public class QuotationCalculatorTest
{
	

	[Theory]
	[MemberData(nameof(QuotationCalculatorTestData.VatPercentagesData), MemberType = typeof(QuotationCalculatorTestData))]
	public void Should_Return_Vat_Percentage(QuotationSupplementLineDto quotationSupplementLineDto, int expected_vat_percentage)
	{
		var vatPercentage = QuotationCalculator.CalculateSupplementVatPercentage(quotationSupplementLineDto);

		vatPercentage.ShouldBe<int>(expected_vat_percentage);
	}

	[Fact]
	public void Should_Return_Total_Price()
	{
		var totalPrice = QuotationCalculator.CalculateSupplementLineTotalPrice(new QuotationSupplementLineDto()
		{
			Id = 1,
			Name = "foo",
			Description = "bar",
			SupplementPrice = 500M,
			SupplementVat = 105M,
			Quantity = 3
		});

		totalPrice.ShouldBe(new Money(1500));
	}

	[Fact]
	public void Should_Return_Total_Vat()
	{
		var quotationSupplementLineDto = new QuotationSupplementLineDto()
		{
			Id = 1,
			Name = "foo",
			Description = "bar",
			SupplementPrice = 500M,
			SupplementVat = 105M,
			Quantity = 3
		};

		var vatPercentage = 21;

		var totalPrice = new Money(1500);

		QuotationCalculator.CalculateSupplementLineTotalVat(totalPrice, vatPercentage).ShouldBe(new Money(315));
	}

	[Fact]
	public void Should_Return_Formula_Price()
	{
		QuotationModel quotationModel = new(QuotationCalculatorTestData.quotationDtoDetail, QuotationCalculatorTestData.quotationVersionDtoDetail);

		QuotationCalculator.CalculateFormulaPrice(quotationModel).ShouldBe(new Money(1960));
	}

	[Fact]
	public void Should_Return_Formula_Vat()
	{
		QuotationModel quotationModel = new(QuotationCalculatorTestData.quotationDtoDetail, QuotationCalculatorTestData.quotationVersionDtoDetail);

		var formulaPrice = QuotationCalculator.CalculateFormulaPrice(quotationModel);

		QuotationCalculator.CalculateFormulaVat(formulaPrice.Value).ShouldBe(new Money(411.6M));
	}

	[Fact]
	public void Should_Return_Total_Price_Extra_Supplements()
	{
		QuotationCalculator.CalculateExtraSupplementsTotalPrice(QuotationCalculatorTestData.quotationSupplementLineDtos).ShouldBe(new Money(960));
	}

	[Fact]
	public void Should_Return_Total_Vat_Extra_Supplements()
	{
		QuotationCalculator.CalculateExtraSupplementsTotalVat(QuotationCalculatorTestData.quotationSupplementLineDtos).ShouldBe(new Money(201.6M));
	}
}
