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
            var product = new Product
            {
                ID = (long) row["varenummer"],
                FirstSeen = (DateTime) row["first_seen"],
                LastSeen = (DateTime) row["last_seen"],
                ProductName = (string) row["varenavn"],
                Volume = (double) row["volum"],
                Price = (decimal) row["pris"],
                ProductType = DatabaseMappingUtil.ConvertFromDBVal<string>(row["varetype"]),
                ProductSelection = (string) row["produktutvalg"],
                StoreCategory = (string) row["butikkategori"],
                Fylde = (byte) row["fylde"],
                Freshness = (byte) row["friskhet"],
                Garvestoffer = (byte) row["garvestoffer"],
                Bitterness = (byte) row["bitterhet"],
                Sweetness = (byte) row["sodme"],
                Color = DatabaseMappingUtil.ConvertFromDBVal<string>(row["farge"]),
                Smell = DatabaseMappingUtil.ConvertFromDBVal<string>(row["lukt"]),
                Taste = DatabaseMappingUtil.ConvertFromDBVal<string>(row["smak"]),
                FitsWith1 = DatabaseMappingUtil.ConvertFromDBVal<string>(row["passer_til_1"]),
                FitsWith2 = DatabaseMappingUtil.ConvertFromDBVal<string>(row["passer_til_2"]),
                FitsWith3 = DatabaseMappingUtil.ConvertFromDBVal<string>(row["passer_til_3"]),
                Country = (string) row["land"],
                District = (string) row["distrikt"],
                Subdistrict = DatabaseMappingUtil.ConvertFromDBVal<string>(row["underdistrikt"]),
                Year = DatabaseMappingUtil.ConvertFromDBValNullable<short>(row["argang"]),
                RawMaterial = DatabaseMappingUtil.ConvertFromDBVal<string>(row["rastoff"]),
                Method = DatabaseMappingUtil.ConvertFromDBVal<string>(row["metode"]),
                Alcohol = (double) row["alkohol"],
                Sugar = DatabaseMappingUtil.ConvertFromDBVal<string>(row["sukker"]),
                Acid = DatabaseMappingUtil.ConvertFromDBValNullable<double>(row["syre"]),
                Lagringsgrad = DatabaseMappingUtil.ConvertFromDBVal<string>(row["lagringsgrad"]),
                Producer = DatabaseMappingUtil.ConvertFromDBVal<string>(row["produsent"]),
                Grossist = (string) row["grossist"],
                Distributor = (string) row["distributor"],
                Emballasjetype = (string) row["emballasjetype"],
                Korktype = DatabaseMappingUtil.ConvertFromDBVal<string>(row["korktype"]),
                Ecologic = (bool)row["okologisk"],
                Biodynamic = (bool)row["biodynamisk"],
                Fairtrade = (bool)row["fairtrade"],
                EnvironmentSmartPackaging = (bool)row["miljosmart_emballasje"],
                LowInGluten = (bool)row["gluten_lav_pa"],
                Kosher = (bool)row["kosher"],
                MainGTIN = DatabaseMappingUtil.ConvertFromDBVal<string>(row["hoved_gtin"]),
                OtherGTINs = DatabaseMappingUtil.ConvertFromDBVal<string>(row["andre_gtiner"]),
            };

            return product;
        }
    }
}
