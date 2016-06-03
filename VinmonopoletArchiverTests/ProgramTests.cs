using Microsoft.VisualStudio.TestTools.UnitTesting;
using VinmonopoletArchiver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VinmonopoletArchiver.Entities;

namespace VinmonopoletArchiver.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void GetNewProductsTest()
        {
            List<Product> products = new List<Product>
            {
                new Product {ID = 1101},
                new Product {ID = 16000 },
                new Product {ID =  3902}
            };

            List<Product> newProducts = Program.GetNewProducts(products).ToList();
            Assert.AreEqual(1, newProducts.Count);
            Assert.AreEqual(16000, newProducts[0].ID);
        }
    }
}