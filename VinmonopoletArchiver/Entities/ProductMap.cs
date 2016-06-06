using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace VinmonopoletArchiver.Entities
{
    internal sealed class ProductMap : CsvClassMap<Product>
    {
        public ProductMap()
        {
            Map(m => m.TimeAcquired).Index(0);
            Map(m => m.ID).Index(1);
            Map(m => m.ProductName).Index(2);
            Map(m => m.Volume).Index(3);
            Map(m => m.Price).Index(4);
            // 5 is price/liter
            Map(m => m.ProductType).Index(6).Default(null); // Shouldn't really be allowed to be null, but real world data has it
            Map(m => m.ProductSelection).Index(7);
            Map(m => m.StoreCategory).Index(8);
            Map(m => m.Fylde).Index(9);
            Map(m => m.Freshness).Index(10);
            Map(m => m.Garvestoffer).Index(11);
            Map(m => m.Bitterness).Index(12);
            Map(m => m.Sweetness).Index(13);
            Map(m => m.Color).Index(14).Default(null);
            Map(m => m.Smell).Index(15).Default(null);
            Map(m => m.Taste).Index(16).Default(null);
            Map(m => m.FitsWith1).Index(17).Default(null);
            Map(m => m.FitsWith2).Index(18).Default(null);
            Map(m => m.FitsWith3).Index(19).Default(null);
            Map(m => m.Country).Index(20);
            Map(m => m.District).Index(21);
            Map(m => m.Subdistrict).Index(22).Default(null);
            Map(m => m.Year).Index(23).Default(null);
            Map(m => m.RawMaterial).Index(24).Default(null);
            Map(m => m.Method).Index(25).Default(null);
            Map(m => m.Alcohol).Index(26);
            Map(m => m.Sugar).Index(27).TypeConverter<DoubleOrUnknownConverter>();
            Map(m => m.Acid).Index(28).TypeConverter<DoubleOrUnknownConverter>();
            Map(m => m.Lagringsgrad).Index(29).Default(null);
            Map(m => m.Producer).Index(30).Default(null);
            Map(m => m.Grossist).Index(31);
            Map(m => m.Distributor).Index(32);
            Map(m => m.Emballasjetype).Index(33);
            Map(m => m.Korktype).Index(34).Default(null); // Shouldn't really be allowed to be null, but real world data has it
        }
    }
}
