using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class GetOrderListResponse : IResource
    {
        #region IResource Stuff

        private const string ResourceNameFormatString = "/order/rest/orderlist/{accountId}";
        private const string SandboxResourceNameFormatString = "/order/sandbox/rest/orderlist/{accountId}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }

        #endregion
    }
}
