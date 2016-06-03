using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace VinmonopoletArchiver.Database
{
    /// <summary>
    /// Wrapper class for MySqlCommand.
    /// </summary>
    internal class MySqlCommandWrapper : IDisposable
    {
        private readonly MySqlConnection _connection;
        private readonly MySqlCommand _command;

        public string CommandText
        {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        public MySqlCommandWrapper(MySqlConnection connection)
        {
            _connection = connection;
            _command = connection.CreateCommand();
        }

        /// <summary>
        /// Opens the connection if not already open.
        /// </summary>
        private void Connect()
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

        /// <summary>
        /// Clears the current parameters of the command.
        /// </summary>
        private void ClearCommand()
        {
            _command.Parameters.Clear();
        }

        /// <summary>
        /// Executes a <b>parameterless</b> SQL query against a connection object.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteSimpleNonQuery(string query)
        {
            Connect();
            ClearCommand();

            _command.CommandText = query;
            return _command.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a parameter with value to the current statement.
        /// </summary>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="value">The value of the parameter.</param>
        /// <returns></returns>
        public MySqlParameter AddParameterWithValue(string parameterName, object value)
        {
            return _command.Parameters.AddWithValue(parameterName, value);
        }

        /// <summary>
        /// Executes a SQL statement against a connection object.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery()
        {
            Connect();
            return _command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a INSERT SQL statement against a connection object.
        /// </summary>
        /// <returns>The auto incremented ID.</returns>
        public long ExecuteInsert()
        {
            Connect();
            _command.ExecuteNonQuery();
            return _command.LastInsertedId;
        }

        public bool GetExists()
        {
            Connect();
            return (int) _command.ExecuteScalar() != 0;
        }

        public int GetIntegerValue()
        {
            Connect();
            return (int) _command.ExecuteScalar();
        }

        public DataTable GetDataTable()
        {
            DataTable table = new DataTable();
            Connect();

            using (MySqlDataAdapter dataAdapter = new MySqlDataAdapter(_command))
            {
                dataAdapter.Fill(table);
            }
            return table;
        }

        public void Dispose()
        {
            _command.Dispose();
            _connection.Dispose();
        }
    }
}
