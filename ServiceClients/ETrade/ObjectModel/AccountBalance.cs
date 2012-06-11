using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class AccountBalanceResponse : IResource
    {
        #region IResource Stuff

        private const string ResourceNameFormatString = "/accounts/rest/accountbalance/{accountId}";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountbalance/{accountId}";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }

        #endregion
    }
}
