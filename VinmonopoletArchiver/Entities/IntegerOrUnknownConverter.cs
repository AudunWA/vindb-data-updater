using System;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace VinmonopoletArchiver.Entities
{
    internal class IntegerOrUnknownConverter : ITypeConverter
    {
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            int? test = null;

            //MySQL doesn't seem to support NaN, use null instead
            if (int.TryParse(text, out int value))
                test = value;
            return test;
            //return double.TryParse(text, out value) ? value : -1d;
        }

        public bool CanConvertFrom(Type type)
        {
            return type == typeof (string);
        }

        public bool CanConvertTo(Type type)
        {
            return type == typeof (int?);
        }
    }
}