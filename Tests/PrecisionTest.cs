
namespace Sappworks.Stocks.Tests
{
    using Sappworks.Stocks.Numerics;
    
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrecisionTest
    {
        [TestMethod]
        public void AlmostEqualsTest()
        {
            Assert.IsTrue(1.1d.AlmostEquals(1.0999999999999999999999, .00000000000000000000011));
        }
    }
}
