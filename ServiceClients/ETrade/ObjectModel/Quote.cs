using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class QuoteResponse : IResource
    {
        private const string ResourceNameFormatString = "/market/rest/quote/{symbols}?detailFlag={quoteType}";
        private const string SandboxResourceNameFormatString = "/market/sandbox/rest/quote/{symbols}?detailFlag={quoteType}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }
    }
}
