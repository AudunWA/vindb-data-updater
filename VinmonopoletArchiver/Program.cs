using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using VinmonopoletArchiver.Database;
using VinmonopoletArchiver.Database.Util;
using VinmonopoletArchiver.Entities;

namespace VinmonopoletArchiver
{
    public class Program
    {
        private const string PRODUCTS_URL = "http://www.vinmonopolet.no/medias/sys_master/products/products/hbc/hb0/8834253127710/produkter.csv";
        private const string STORES_URL = "http://www.vinmonopolet.no/medias/sys_master/locations/locations/h3c/h4a/8834253946910.csv";

        private static void Main(string[] args)
        {
            // Required to send HTTP requests to vinmonopolet.no
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;

            //foreach (string file in Directory.GetFiles("data"))
            //{
            //    if (file.Contains("produkter"))
            //        InsertFileIntoDb(file.Substring(5));
            //}
            //return;
            Directory.CreateDirectory("data");
            using (WebClient client = new WebClient())
            {
                byte[] productsBytes = client.DownloadData(PRODUCTS_URL);
                Dictionary<long, Product> products = GetProducts(productsBytes);
                if (products.Count > 0)
                {
                    DateTime time = products.Values.ElementAt(0).TimeAcquired; // Time only specified for first item

                    // Insert these products into database
                    //InsertProductsIntoDb(GetProducts(productsBytes));
                    StartImport(products, time);
                    File.WriteAllBytes("data\\" + "produkter" + time.ToString("yyyyMMdd-HHmmss-fff") + ".csv", productsBytes);
                    //File.WriteAllBytes("data\\" + (GetFileName(client) ?? "produkter" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".csv"), productsBytes);
                }

                byte[] storesBytes = client.DownloadData(STORES_URL);
                File.WriteAllBytes("data\\" + "butikker" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".csv", storesBytes);
                //File.WriteAllBytes("data\\" + (GetFileName(client) ?? "butikker" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".csv"), storesBytes);
            }
        }

        // Try to extract the filename from the Content-Disposition header
        private static string GetFileName(WebClient client)
        {
            if (!string.IsNullOrEmpty(client.ResponseHeaders["Content-Disposition"]))
                return client.ResponseHeaders["Content-Disposition"].Substring(client.ResponseHeaders["Content-Disposition"].IndexOf("filename=", StringComparison.InvariantCulture) + 10).Replace("\"", "");

            return null;
        }

        private static Dictionary<long, Product> GetProducts(string fileName)
        {
            using (FileStream fileStream = new FileStream($"data\\{fileName}", FileMode.Open))
            {
                using (TextReader textReader = new StreamReader(fileStream, Encoding.UTF7))
                {
                    using (CsvReader csvReader = new CsvReader(textReader))
                    {
                        csvReader.Configuration.Delimiter = ";";
                        csvReader.Configuration.RegisterClassMap<ProductMap>();
                        return csvReader.GetRecords<Product>().ToDictionary(p => p.ID, p => p);
                    }
                }
            }
        }

        private static Dictionary<long, Product> GetProducts(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                using (TextReader textReader = new StreamReader(stream, Encoding.UTF7))
                {
                    using (CsvReader csvReader = new CsvReader(textReader))
                    {
                        csvReader.Configuration.Delimiter = ";";
                        csvReader.Configuration.RegisterClassMap<ProductMap>();
                        return csvReader.GetRecords<Product>().ToDictionary(p => p.ID, p => p);
                    }
                }
            }
        }

        private static void StartImport(IDictionary<long, Product> products, DateTime time)
        {
            // First check if already imported
            if (CheckChangeRegistered(time))
            {
                // TODO: Log
                return;
            }

            // Insert the change itself
            int changeID = CreateChange(time);
            if (changeID < 1)
            {
                // TODO: Log
                return;
            }

            // Process and insert all products etc
            ProcessProducts(products);

            // Mark the change as completed
            SetChangeEndTime(changeID);
        }

        private static bool SetChangeEndTime(int changeID)
        {
            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                command.CommandText = "UPDATE change_log SET end_import = NOW() WHERE change_id = @id";
                command.AddParameterWithValue("id", changeID);
                int affectedRows = command.ExecuteNonQuery();

                if (affectedRows != 1)
                {
                    // TODO: Log
                    return false;
                }
                return true;
            }
        }

        private static int CreateChange(DateTime time)
        {
            int changeID;
            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                command.CommandText = "INSERT INTO change_log(time,start_import) VALUES(@time,NOW())";
                command.AddParameterWithValue("time", time);
                changeID = (int) command.ExecuteInsert();
            }
            return changeID;
        }

        private static bool CheckChangeRegistered(DateTime time)
        {
            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                command.CommandText = "SELECT COUNT(1) FROM change_log WHERE time = @time";
                command.AddParameterWithValue("time", time);
                return command.GetExists();
            }
        }

        public static IEnumerable<Product> GetNewProducts(IList<Product> products)
        {
            List<long> existingIDs = new List<long>();
            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                command.CommandText = "SELECT varenummer FROM product WHERE FIND_IN_SET(varenummer,@ids) != 0";
                command.AddParameterWithValue("ids", string.Join(",", products.Select(p => p.ID).ToArray()));
                DataTable table = command.GetDataTable();

                foreach (DataRow row in table.Rows)
                {
                    existingIDs.Add((long)row[0]);
                }
            }
            return products.Where(p => !existingIDs.Contains(p.ID));
        }

        private static void ProcessProducts(IDictionary<long, Product> products)
        {
            // TODO: Redo in following way:
            // 1. Retrieve and map all products from db
            // 2. Compare with the ones mapped from csv
            // 3a. REPLACE INTO everything?
            // 3b. INSERT new, UPDATE old ones
            // 4. For existing products, INSERT into product_change
            Dictionary<long, Product> databaseProducts = ProductFactory.FetchAllProducts();
            List<ProductChange> productChanges = new List<ProductChange>();

            foreach (Product dbProduct in databaseProducts.Values)
            {
                Product csvProduct;
                if (products.TryGetValue(dbProduct.ID, out csvProduct))
                {
                    // CSV product exists in database
                    // Compare CSV and DB product and run db updates (insert into product_change)
                    List<ProductChange> changes = dbProduct.FindChanges(csvProduct);
                    if (changes.Count > 0)
                    {
                        productChanges.AddRange(changes);
                    }
                    else
                    {
                        // Product is identical, remove from database insert dictionary
                        products.Remove(dbProduct.ID);
                    }
                }
                else
                {
                    // CSV product is not in database, probably new
                }
            }

            InsertProductsIntoDb(products);

            //List<Product> newProducts = GetNewProducts(products).ToList();
            //int insertCount = InsertProductsIntoDb(newProducts);
            //if (insertCount != newProducts.Count)
            //{
            //    // TODO: Log
            //    return;
            //}

            //// Remove the new product so we don't run an update query later
            //products.RemoveAll(p => newProducts.Contains(p));


        }

        private static int InsertProductsIntoDb(IDictionary<long, Product> products)
        {
            QueryChunk insertQuery = new QueryChunk("REPLACE INTO product VALUES");

            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                int i = 0;
                foreach (Product product in products.Values)
                {
                    product.TimeAcquired = products.Values.ElementAt(0).TimeAcquired; // Time only specified for first item
                    insertQuery.AddQuery(string.Format("(@id{0},@time{0},@time{0},@name{0},@volume{0},@price{0},@type{0},@selection{0},@category{0},@fylde{0},@freshness{0}" +
                                                       ",@garvestoffer{0},@bitterness{0},@sweetness{0},@color{0},@smell{0},@taste{0},@fw1{0},@fw2{0},@fw3{0}" +
                                                       ",@country{0},@district{0},@subdistrict{0},@year{0},@rawmaterial{0},@method{0},@alcohol{0},@sugar{0},@acid{0}" +
                                                       ",@lagringsgrad{0},@producer{0},@grossist{0},@distributor{0},@emballasjetype{0},@korktype{0})", i));

                    insertQuery.AddParameter($"id{i}", product.ID);
                    insertQuery.AddParameter($"time{i}", product.TimeAcquired);
                    insertQuery.AddParameter($"name{i}", product.ProductName);
                    insertQuery.AddParameter($"volume{i}", product.Volume);
                    insertQuery.AddParameter($"price{i}", product.Price);
                    insertQuery.AddParameter($"type{i}", product.ProductType);
                    insertQuery.AddParameter($"selection{i}", product.ProductSelection);
                    insertQuery.AddParameter($"category{i}", product.StoreCategory);
                    insertQuery.AddParameter($"fylde{i}", product.Fylde);
                    insertQuery.AddParameter($"freshness{i}", product.Freshness);
                    insertQuery.AddParameter($"garvestoffer{i}", product.Garvestoffer);
                    insertQuery.AddParameter($"bitterness{i}", product.Bitterness);
                    insertQuery.AddParameter($"sweetness{i}", product.Sweetness);
                    insertQuery.AddParameter($"color{i}", product.Color);
                    insertQuery.AddParameter($"smell{i}", product.Smell);
                    insertQuery.AddParameter($"taste{i}", product.Taste);
                    insertQuery.AddParameter($"fw1{i}", product.FitsWith1);
                    insertQuery.AddParameter($"fw2{i}", product.FitsWith2);
                    insertQuery.AddParameter($"fw3{i}", product.FitsWith3);
                    insertQuery.AddParameter($"country{i}", product.Country);
                    insertQuery.AddParameter($"district{i}", product.District);
                    insertQuery.AddParameter($"subdistrict{i}", product.Subdistrict);
                    insertQuery.AddParameter($"year{i}", product.Year);
                    insertQuery.AddParameter($"rawmaterial{i}", product.RawMaterial);
                    insertQuery.AddParameter($"method{i}", product.Method);
                    insertQuery.AddParameter($"alcohol{i}", product.Alcohol);
                    insertQuery.AddParameter($"sugar{i}", product.Sugar);
                    insertQuery.AddParameter($"acid{i}", product.Acid);
                    insertQuery.AddParameter($"lagringsgrad{i}", product.Lagringsgrad);
                    insertQuery.AddParameter($"producer{i}", product.Producer);
                    insertQuery.AddParameter($"grossist{i}", product.Grossist);
                    insertQuery.AddParameter($"distributor{i}", product.Distributor);
                    insertQuery.AddParameter($"emballasjetype{i}", product.Emballasjetype);
                    insertQuery.AddParameter($"korktype{i}", product.Korktype);
                    i++;
                }

                return insertQuery.Execute(command);
            }
        }
    }
}
