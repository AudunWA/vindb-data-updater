using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinmonopoletArchiver.Database.Util
{
    public class DatabaseMappingUtil
    {
        // http://stackoverflow.com/questions/870697/unable-to-cast-object-of-type-system-dbnull-to-type-system-string
        public static T ConvertFromDBVal<T>(object obj) where T : class
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default; // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }

        public static T? ConvertFromDBValNullable<T>(object obj) where T : struct
        {
            if (obj == null || obj == DBNull.Value)
            {
                return null;
            }
            else
            {
                return (T) obj;
            }
        }
    }
}
