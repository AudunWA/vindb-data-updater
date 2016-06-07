using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using VinmonopoletArchiver.Database;
using VinmonopoletArchiver.Database.Util;

namespace VinmonopoletArchiver.Entities
{
    public static class ProductFactory
    {
        public static Dictionary<long, Product> FetchAllProducts()
        {
            Dictionary<long, Product> products = new Dictionary<long, Product>();
            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                command.CommandText = "SELECT * FROM product";
                DataTable table = command.GetDataTable();
                foreach (DataRow row in table.Rows)
                {
                    Product product = Get(row);
                    products.Add(product.ID, product);
                }
            }
            return products;
        } 
        public static Product Get(DataRow row)
        {
            Product product = new Product();
            product.ID = (long) row["varenummer"];
            product.FirstSeen = (DateTime) row["first_seen"];
            product.LastSeen = (DateTime) row["last_seen"];
            product.ProductName = (string) row["varenavn"];
            product.Volume = (double) row["volum"];
            product.Price = (double) row["pris"];
            product.ProductType = DatabaseMappingUtil.ConvertFromDBVal<string>(row["varetype"]);
            product.ProductSelection = (string) row["produktutvalg"];
            product.StoreCategory = (string) row["butikkategori"];
            product.Fylde = (byte) row["fylde"];
            product.Freshness = (byte) row["friskhet"];
            product.Garvestoffer = (byte) row["garvestoffer"];
            product.Bitterness = (byte) row["bitterhet"];
            product.Sweetness = (byte) row["sodme"];
            product.Color = DatabaseMappingUtil.ConvertFromDBVal<string>(row["farge"]);
            product.Smell = DatabaseMappingUtil.ConvertFromDBVal<string>(row["lukt"]);
            product.Taste = DatabaseMappingUtil.ConvertFromDBVal<string>(row["smak"]);
            product.FitsWith1 = DatabaseMappingUtil.ConvertFromDBVal<string>(row["passer_til_1"]);
            product.FitsWith2 = DatabaseMappingUtil.ConvertFromDBVal<string>(row["passer_til_2"]);
            product.FitsWith3 = DatabaseMappingUtil.ConvertFromDBVal<string>(row["passer_til_3"]);
            product.Country = (string) row["land"];
            product.District = (string) row["distrikt"];
            product.Subdistrict = DatabaseMappingUtil.ConvertFromDBVal<string>(row["underdistrikt"]);
            product.Year = DatabaseMappingUtil.ConvertFromDBValNullable<int>(row["argang"]);
            product.RawMaterial = DatabaseMappingUtil.ConvertFromDBVal<string>(row["rastoff"]);
            product.Method = DatabaseMappingUtil.ConvertFromDBVal<string>(row["metode"]);
            product.Alcohol = (double) row["alkohol"];
            product.Sugar = DatabaseMappingUtil.ConvertFromDBValNullable<double>(row["sukker"]);
            product.Acid = DatabaseMappingUtil.ConvertFromDBValNullable<double>(row["syre"]);
            product.Lagringsgrad = DatabaseMappingUtil.ConvertFromDBVal<string>(row["lagringsgrad"]);
            product.Producer = DatabaseMappingUtil.ConvertFromDBVal<string>(row["produsent"]);
            product.Grossist = (string) row["grossist"];
            product.Distributor = (string) row["distributor"];
            product.Emballasjetype = (string) row["emballasjetype"];
            product.Korktype = DatabaseMappingUtil.ConvertFromDBVal<string>(row["korktype"]);

            return product;
        }
    }
}
