using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    public interface IResource
    {
        string GetResourceName(bool productionMode);
    }
}
