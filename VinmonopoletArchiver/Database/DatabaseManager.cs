using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace VinmonopoletArchiver.Database
{
    internal static class DatabaseManager
    {
        private static readonly string ConnectionString;

        static DatabaseManager()
        {
            ConnectionString = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Port = 3306,
                UserID = "root",
                Password = "",
                Database = "pol",
                ConvertZeroDateTime = true,
            }.GetConnectionString(true);
        }

        public static MySqlConnection CreateConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public static MySqlCommandWrapper CreateCommand()
        {
            return new MySqlCommandWrapper(CreateConnection());
        }
    }
}
