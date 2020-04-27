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
        public static CultureInfo CultureInfo = CultureInfo.GetCultureInfo("nb-NO");
        public static Encoding CurrentEncoding = Encoding.GetEncoding(CultureInfo.TextInfo.ANSICodePage);

        private const string PRODUCTS_URL =
            "https://www.vinmonopolet.no/medias/sys_master/products/products/hbc/hb0/8834253127710/produkter.csv";

        private const string STORES_URL =
            "https://www.vinmonopolet.no/medias/sys_master/locations/locations/h3c/h4a/8834253946910/8834253946910.csv";

        public static void Main(string[] args)
        {
            Console.WriteLine("Vinmonopolet archiver starting..");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (args.Length > 0 && args[0] == "-s")
            {
                Console.WriteLine("Starting import...");
                string fileName;
                Dictionary<long, Product> products = DownloadLatestCSV(out fileName);
                if (products == null)
                {
                    Console.WriteLine("Didn't find any products..");
                    // TODO: Log
                    return;
                }

                StartImport(products, fileName);
                return;
            }

            string command;

            do
            {
                command = Console.ReadLine();
                string[] commandParameters = command.Split(' ');
                switch (commandParameters[0])
                {
                    case "insert":
                    {
                        string fileName = commandParameters[1];
                        Dictionary<long, Product> products = GetProducts(fileName);
                        StartImport(products, fileName);
                        break;
                    }
                    case "download":
                    {
                        string fileName;
                        DownloadLatestCSV(out fileName);
                        break;
                    }

                    case "dlinsert":
                    {
                        string fileName;
                        Dictionary<long, Product> products = DownloadLatestCSV(out fileName);
                        if (products == null)
                        {
                            // TODO: Log
                            return;
                        }

                        StartImport(products, fileName);
                        break;
                    }
                    case "insertall":
                    {
                        string[] orderedFiles =
                            Directory.GetFiles("data").Where(f => f.Contains("produkter")).OrderBy(s => s).ToArray();
                        foreach (string file in orderedFiles)
                        {
                            Dictionary<long, Product> products = GetProducts(file.Substring(5));
                            StartImport(products, file.Substring(5));
                        }
                        break;
                    }
                    default:
                        Console.WriteLine("Invalid command: " + commandParameters[0]);
                        break;
                }
            } while (command != "exit");
        }

        private static Dictionary<long, Product> DownloadLatestCSV(out string fileName)
        {
            Directory.CreateDirectory("data");
            using (WebClient client = new WebClient())
            {
                byte[] productsBytes = client.DownloadData(PRODUCTS_URL);
                Dictionary<long, Product> products = GetProducts(productsBytes);

                //if (products.Count > 0)
                //{
                    //DateTime time = products.Values.ElementAt(0).TimeAcquired; // Time only specified for first item
                    //fileName = "produkter" + time.ToString("yyyyMMdd-HHmmss-fff") + ".csv";
                //}
                //else
                //{
                    //fileName = "produkter" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".corrupt.csv";
                    fileName = "produkter" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".csv";
                //}

                File.WriteAllBytes("data\\" + fileName, productsBytes);
                Console.WriteLine("Successfully downloaded " + fileName);

                byte[] storesBytes = client.DownloadData(STORES_URL);
                File.WriteAllBytes("data\\" + "butikker" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") + ".csv",
                    storesBytes);
                Console.WriteLine("Successfully downloaded " + "butikker" + DateTime.Now.ToString("yyyyMMdd-HHmmss-fff") +
                                  ".csv");
                return products.Count > 0 ? products : null;
            }
        }

        // Try to extract the filename from the Content-Disposition header
        private static string GetFileName(WebClient client)
        {
            if (!string.IsNullOrEmpty(client.ResponseHeaders["Content-Disposition"]))
                return
                    client.ResponseHeaders["Content-Disposition"].Substring(
                        client.ResponseHeaders["Content-Disposition"].IndexOf("filename=",
                            StringComparison.InvariantCulture) + 10).Replace("\"", "");

            return null;
        }

        private static Dictionary<long, Product> GetProducts(string fileName)
        {
            using (FileStream fileStream = new FileStream($"data\\{fileName}", FileMode.Open))
            {
                using (TextReader textReader = new StreamReader(fileStream, CurrentEncoding))
                {
                    using (CsvReader csvReader = new CsvReader(textReader))
                    {
                        csvReader.Configuration.HeaderValidated = null;
                        csvReader.Configuration.Delimiter = ";";
                        csvReader.Configuration.RegisterClassMap<ProductMap>();
                        return csvReader.GetRecords<Product>().Where(p => !double.IsNaN(p.Alcohol)).ToDictionary(p => p.ID, p => p);
                    }
                }
            }
        }

        private static Dictionary<long, Product> GetProducts(byte[] data)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    using (TextReader textReader = new StreamReader(stream, CurrentEncoding))
                    {
                        using (CsvReader csvReader = new CsvReader(textReader))
                        {
                            csvReader.Configuration.HeaderValidated = null;
                            csvReader.Configuration.Delimiter = ";";
                            csvReader.Configuration.RegisterClassMap<ProductMap>();
                            return csvReader.GetRecords<Product>().Where(p => !double.IsNaN(p.Alcohol)).ToDictionary(p => p.ID, p => p);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Parse error: " + e);
                return new Dictionary<long, Product>();
            }
        }

        private static void StartImport(IDictionary<long, Product> products, string fileName)
        {
            DateTime time = DateTime.Now; // Time only specified for first item
            try
            {
                time = products.Values.ElementAt(0).TimeAcquired;
                if(time == default(DateTime)) // Invalid time, some csv files has this issue
                    time = DateTime.ParseExact(fileName.Replace("produkter", "").Replace(".csv", ""), "yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                time = DateTime.ParseExact(fileName.Replace("produkter", "").Replace(".csv", ""), "yyyyMMdd-HHmmss-fff", CultureInfo.InvariantCulture);
            }
            //DateTime time = DateTime.Now; // Timestamp removed from files
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
            ProcessProducts(products, changeID);

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

        private static void ProcessProducts(IDictionary<long, Product> products, int changeID)
        {
            // TODO: Redo in following way:
            // 1. Retrieve and map all products from db
            // 2. Compare with the ones mapped from csv
            // 3a. REPLACE INTO everything?
            // 3b. INSERT new, UPDATE old ones
            // 4. For existing products, INSERT into product_change
            Dictionary<long, Product> databaseProducts = ProductFactory.FetchAllProducts();
            List<ProductChange> productChanges = new List<ProductChange>();
            DateTime time = products.Values.ElementAt(0).TimeAcquired; // Time only specified for first item

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
                        //// Product is identical, remove from database insert dictionary
                        //products.Remove(dbProduct.ID);
                    }
                    // Set first_seen to existing first_seen
                    csvProduct.FirstSeen = dbProduct.FirstSeen;
                }
                else
                {
                    // CSV product is not in database, probably new
                }
            }

            InsertProductsIntoDb(products);
            InsertChangesIntoDB(productChanges, changeID);

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

        private static int InsertChangesIntoDB(IList<ProductChange> productChanges, int changeID)
        {
            if (productChanges.Count == 0)
                return 0;
            QueryChunk insertQuery = new QueryChunk("INSERT INTO product_change VALUES");

            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                for (int i = 0; i < productChanges.Count; i++)
                {
                    ProductChange change = productChanges[i];
                    insertQuery.AddQuery(string.Format("(@id{0},@field{0},@changeid{0},@oldvalue{0},@newvalue{0})", i));
                    insertQuery.AddParameter($"id{i}", change.ProductID);
                    insertQuery.AddParameter($"field{i}", change.ChangedField);
                    insertQuery.AddParameter($"changeid{i}", changeID); // TODO: Set as global parameter
                    insertQuery.AddParameter($"oldvalue{i}", change.OldValue);
                    insertQuery.AddParameter($"newvalue{i}", change.NewValue);
                }
                return insertQuery.Execute(command);
            }
        }

        private static int InsertProductsIntoDb(IDictionary<long, Product> products)
        {
            QueryChunk insertQuery = new QueryChunk("REPLACE INTO product VALUES");

            using (MySqlCommandWrapper command = DatabaseManager.CreateCommand())
            {
                // Temp hack to let us bypass foreign key checks on REPLACE INTO
                command.CommandText = "SET FOREIGN_KEY_CHECKS=0";
                command.ExecuteNonQuery();

                DateTime time = products.Values.ElementAt(0).TimeAcquired; // Time only specified for first item

                int i = 0;
                foreach (Product product in products.Values)
                {
                    if (product.FirstSeen == default(DateTime)) // New product, set first seen to now
                        product.FirstSeen = time;

                    product.LastSeen = time;
                    product.TimeAcquired = time;

                    insertQuery.AddQuery(string.Format("(@id{0},@firstseen{0},@lastseen{0},@name{0},@volume{0},@price{0},@type{0},@selection{0},@category{0},@fylde{0},@freshness{0}" +
                                                       ",@garvestoffer{0},@bitterness{0},@sweetness{0},@color{0},@smell{0},@taste{0},@fw1{0},@fw2{0},@fw3{0}" +
                                                       ",@country{0},@district{0},@subdistrict{0},@year{0},@rawmaterial{0},@method{0},@alcohol{0},@sugar{0},@acid{0}" +
                                                       ",@lagringsgrad{0},@producer{0},@grossist{0},@distributor{0},@emballasjetype{0},@korktype{0})", i));

                    insertQuery.AddParameter($"id{i}", product.ID);
                    insertQuery.AddParameter($"firstseen{i}", product.FirstSeen);
                    insertQuery.AddParameter($"lastseen{i}", product.LastSeen);
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
