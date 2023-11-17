using Foodtruck.Shared.Quotations;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foodtruck.Shared.Pdfs
{
    public interface IPdfService
    {
        Task<string> GetQuotationPdfAsBase64(QuotationDto.Detail quotation);
    }
}
