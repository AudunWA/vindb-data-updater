using System.Collections.Generic;
using System.Text;

namespace VinmonopoletArchiver.Database.Util
{
    internal class SmallChunk
    {
        public const int MAX_ALLOWED_PACKET_SIZE = 1048576;

        private StringBuilder _queries;
        internal Dictionary<string, object> Parameters;

        private EndingType _endingType;
        private int _queryCount;
        private string _endQuery;
        internal SmallChunk()
        {
            _queries = new StringBuilder();
            Parameters = new Dictionary<string, object>();
            _endQuery = "";
            _endingType = EndingType.Sequential;
        }

        internal SmallChunk(string startQuery, string endQuery)
        {
            _queries = new StringBuilder(startQuery);
            Parameters = new Dictionary<string, object>();
            _endQuery = endQuery;
            _endingType = EndingType.Continuous;
        }

        internal SmallChunk(string startQuery)
        {
            _queries = new StringBuilder(startQuery);
            Parameters = new Dictionary<string, object>();
            _endQuery = "";
            _endingType = EndingType.Continuous;
        }

        internal bool HasCapacity(string query)
        {
            return _queries.Length + query.Length + _endQuery.Length <= MAX_ALLOWED_PACKET_SIZE / 2;
        }

        internal void AddQuery(string query)
        {
            _queryCount++;
            _queries.Append(query);

            switch (_endingType)
            {
                case EndingType.Continuous:
                    _queries.Append(",");
                    break;
                case EndingType.Sequential:
                    _queries.Append(";");
                    break;
            }
        }

        public string GetQueryString()
        {
            if (_queryCount == 0)
                return "";

            string queries = _queries.Remove(_queries.Length - 1, 1).ToString();

            if (!string.IsNullOrWhiteSpace(_endQuery))
                queries += _endQuery;

            return queries;
        }

        internal void Dispose()
        {
            _queries.Clear();
            Parameters.Clear();
            _queries = null;
            Parameters = null;
        }
    }
}