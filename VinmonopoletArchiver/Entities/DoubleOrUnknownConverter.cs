using System;
using CsvHelper.TypeConversion;

namespace VinmonopoletArchiver.Entities
{
    internal class DoubleOrUnknownConverter : ITypeConverter
    {
        public string ConvertToString(TypeConverterOptions options, object value)
        {
            return value.ToString();
        }

        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            double value;
            double? test = null;

            //MySQL doesn't seem to support NaN, use null instead
            if (double.TryParse(text, out value))
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
            return type == typeof (double?);
        }
    }
}