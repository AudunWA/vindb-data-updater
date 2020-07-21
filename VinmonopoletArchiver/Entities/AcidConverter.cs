using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace VinmonopoletArchiver.Entities
{
    internal class AcidConverter : ITypeConverter
    {
        public string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return value.ToString();
        }

        public object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            double? test = null;

            //MySQL doesn't seem to support NaN, use null instead
            if (double.TryParse(text, NumberStyles.Any, new CultureInfo("no"), out double value))
                test = Math.Round(value * 100); // Mutliply by for backwards compatibility with db
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