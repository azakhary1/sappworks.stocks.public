
namespace Sappworks.Stocks
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class AuthenticationException : Exception
    {
        public AuthenticationException() { }
        public AuthenticationException(string message) : base(message) { }
        public AuthenticationException(string message, Exception inner) : base(message, inner) { }
        protected AuthenticationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public string AcceptableVersionTo { get; set; }
        public string AcceptableVersionFrom { get; set; }
        public IEnumerable<string> ParametersRejected { get; set; }
        public IEnumerable<string> ParametersAbsent { get; set; }
        public string ProblemAdvice { get; set; }
        public string Problem { get; set; }
        public DateTime? AcceptableTimeStampsTo { get; set; }
        public DateTime? AcceptableTimeStampsFrom { get; set; }

        public Uri RequestUri { get; set; }
        public IEnumerable<string> AuthorizationHeaders { get; set; }

    }
}
