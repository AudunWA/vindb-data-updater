using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace VinmonopoletArchiver.Database.Util
{
    internal class QueryChunk
    {
        private List<SmallChunk> _chunks;
        private int queryCount;
        private EndingType endingType;
        private string _startQuery;
        private string _endQuery;

        internal int Count
        {
            get { return queryCount; }
        }

        public QueryChunk()
        {
            _chunks = new List<SmallChunk>();
            _chunks.Add(new SmallChunk());
            queryCount = 0;
            endingType = EndingType.Sequential;
        }

        public QueryChunk(string startQuery, string endQuery)
        {
            _chunks = new List<SmallChunk>();
            _chunks.Add(new SmallChunk(startQuery, endQuery));
            endingType = EndingType.Continuous;
            queryCount = 0;
            _startQuery = startQuery;
            _endQuery = endQuery;
        }

        public QueryChunk(string startQuery)
        {
            _chunks = new List<SmallChunk>();
            _chunks.Add(new SmallChunk(startQuery));
            endingType = EndingType.Continuous;
            queryCount = 0;
            _startQuery = startQuery;
            _endQuery = "";
        }

        private SmallChunk GetCurrentChunk()
        {
            return _chunks[_chunks.Count - 1];
        }

        internal void AddQuery(string query)
        {
            if (!GetCurrentChunk().HasCapacity(query)) // Create new chunk
            {
                if (endingType == EndingType.Continuous)
                    _chunks.Add(new SmallChunk(_startQuery, _endQuery));
                else
                    _chunks.Add(new SmallChunk());
            }

            GetCurrentChunk().AddQuery(query);
            queryCount++;
        }

        internal void AddParameter(string parameterName, object value)
        {
            GetCurrentChunk().Parameters.Add(parameterName, value);
        }

        internal int Execute(MySqlCommandWrapper command)
        {
            if (queryCount == 0)
                return 0;

            int totalAffectedRows = 0;

            foreach (SmallChunk chunk in _chunks)
            {
                command.CommandText = chunk.GetQueryString();

                foreach (KeyValuePair<string, object> parameter in chunk.Parameters)
                    command.AddParameterWithValue(parameter.Key, parameter.Value);

                totalAffectedRows += command.ExecuteNonQuery();
                chunk.Dispose();
            }
            return totalAffectedRows;
        }

        internal void Dispose()
        {
            _chunks.Clear();
            _chunks = null;
        }
    }
}
