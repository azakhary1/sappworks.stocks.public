
namespace Stocks.Common
{
    using System;

    [Serializable]
    public class XmlFormatException : Exception
    {
        public string Xml { get; set; }

        public XmlFormatException() { }
        public XmlFormatException(string message) : base(message) { }
        public XmlFormatException(string message, Exception inner) : base(message, inner) { }
        public XmlFormatException(string message, string xml, Exception inner) : base(message, inner) 
        {
            Xml = xml;
        }
        protected XmlFormatException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
