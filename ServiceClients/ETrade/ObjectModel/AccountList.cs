using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks.ServiceClients.ETrade.ObjectModel
{
    partial class AccountListResponse : IResource
    {
        #region IResource Stuff

        private const string ResourceNameFormatString = "/accounts/rest/accountlist";
        private const string SandboxResourceNameFormatString = "/accounts/sandbox/rest/accountlist";

        string IResource.GetResourceName(bool productionMode)
        {
            return productionMode ? ResourceNameFormatString : SandboxResourceNameFormatString;
        }

        #endregion
    }
}
