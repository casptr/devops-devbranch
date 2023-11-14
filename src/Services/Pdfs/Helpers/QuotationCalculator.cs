using Domain.Common;
using Domain.Formulas;
using Foodtruck.Shared.Quotations;
using Foodtruck.Shared.Supplements;
using Services.Pdfs.QuotationPdfs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Pdfs.Helpers;

static class QuotationCalculator
{
    public static int CalculateSupplementVatPercentage(QuotationSupplementLineDto supplementLine)
    {
        return (int) (supplementLine.SupplementVat / supplementLine.SupplementPrice * 100M);
    }

    public static Money CalculateSupplementLineTotalPrice(QuotationSupplementLineDto supplementLine)
    {
        return new Money((supplementLine.SupplementPrice * supplementLine.Quantity));
    }

    public static Money CalculateSupplementLineTotalVat(Money totalprice, int vatPercentage)
    {
        return new Money(totalprice.Value * vatPercentage / 100M);
    }

    public static Money CalculateFormulaPrice(QuotationModel model)
    {
        decimal formulaPrice = model.FoodtruckPrice;
        if (model.FormulaSupplementLines != null)
        {
            foreach (var item in model.FormulaSupplementLines)
            {
                formulaPrice += (item.SupplementPrice * item.Quantity);
            }
            
        }
        return new Money(formulaPrice);
    }

    public static Money CalculateFormulaVat(decimal totalprice)
    {
        var vatPercentage = 21;
        return new Money(totalprice * vatPercentage / 100M);
    }

    internal static Money CalculateExtraSupplementsTotalPrice(IEnumerable<QuotationSupplementLineDto>? extraSupplementLines)
    {
        decimal totalPrice = 0;
        if(extraSupplementLines != null)
        foreach (var item in extraSupplementLines)
        {
            totalPrice += item.SupplementPrice * item.Quantity;
        }
        return new Money(totalPrice);
    }

    internal static Money CalculateExtraSupplementsTotalVat(IEnumerable<QuotationSupplementLineDto>? extraSupplementLines)
    {
        decimal totalVat = 0;
        if (extraSupplementLines != null)
            foreach (var item in extraSupplementLines)
        {
            int itemVat = CalculateSupplementVatPercentage(item);
            Money itemTotalPrice = CalculateSupplementLineTotalPrice(item);
            totalVat += CalculateSupplementLineTotalVat(itemTotalPrice, itemVat).Value;
        }
        return new Money(totalVat);
    }
}
