using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinmonopoletArchiver.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinmonopoletArchiver.Entities.Tests
{
    [TestClass()]
    public class ProductFactoryTests
    {
        [TestMethod()]
        public void FetchAllProductsTest()
        {
            Dictionary<long, Product> products = ProductFactory.FetchAllProducts();
            Assert.AreNotEqual(0, products.Count);
        }
    }
}