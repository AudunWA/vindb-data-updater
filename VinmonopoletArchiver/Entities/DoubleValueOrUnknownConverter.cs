using System;
using System.Globalization;
using CsvHelper.TypeConversion;

namespace VinmonopoletArchiver.Entities
{
    internal class DoubleValueOrUnknownConverter : ITypeConverter
    {
        public string ConvertToString(TypeConverterOptions options, object value)
        {
            return value.ToString();
        }

        public object ConvertFromString(TypeConverterOptions options, string text)
        {
            double value;
            double test = double.NaN;

            //MySQL doesn't seem to support NaN, use null instead
            if (double.TryParse(text, NumberStyles.Any, new CultureInfo("no"), out value))
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
            return type == typeof (double);
        }
    }
}