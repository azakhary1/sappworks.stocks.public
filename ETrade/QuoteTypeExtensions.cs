using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sappworks.Stocks.ETrade
{
    static class QuoteTypeExtensions
    {
        public static string ToEtradeQuoteType(this QuoteType quoteType)
        {
            switch (quoteType)
            { 
                case QuoteType.Fundamental:
                    
                    return "FUNDAMENTAL";

                case QuoteType.All:

                    return "ALL";

                case QuoteType.IntraDay:

                    return "INTRADAY";

                default:

                    throw new NotImplementedException("Unknown Quote Type");
            }
        }
    }
}
