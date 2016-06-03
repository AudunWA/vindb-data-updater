using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinmonopoletArchiver.Entities
{
    internal class Product
    {
        public long ID { get; set; } // varenummer
        public DateTime TimeAcquired { get; set; } = DateTime.Now;// tid_hentet
        public string ProductName { get; set; } // varenavn
        public double Volume { get; set; } // volum
        public double Price { get; set; } // pris
        public string ProductType { get; set; } // varetype
        public string ProductSelection { get; set; } // produktutvalg
        public string StoreCategory { get; set; } // butikkategori
        public byte Fylde { get; set; } // fylde TODO: English
        public byte Freshness { get; set; } // friskhet
        public byte Garvestoffer { get; set; } // garvestoffer TODO: English
        public byte Bitterness { get; set; } // bitterhet
        public byte Sweetness { get; set; } // sodme
        public string Color { get; set; } // farge
        public string Smell { get; set; } // lukt
        public string Taste { get; set; } // smak
        public string FitsWith1 { get; set; } // passer_til_1
        public string FitsWith2 { get; set; } // passer_til_2
        public string FitsWith3 { get; set; } // passer_til_3
        public string Country { get; set; } // land
        public string District { get; set; } // district
        public string Subdistrict { get; set; } // underdistrikt
        public short? Year { get; set; } // argang
        public string RawMaterial { get; set; } // rastoff
        public string Method { get; set; } // metode
        public double Alcohol { get; set; } // alkohol
        public double? Sugar { get; set; } // sukker
        public double? Acid { get; set; } // syre
        public string Lagringsgrad { get; set; } // lagringsgrad TODO: English
        public string Producer { get; set; } // produsent
        public string Grossist { get; set; } // grossist TODO?: English
        public string Distributor { get; set; } // distributor
        public string Emballasjetype { get; set; } // emballasjetype TODO: English
        public string Korktype { get; set; } // korktype TODO: English

        public override string ToString()
        {
            return $"ID: {ID}, TimeAcquired: {TimeAcquired}, ProductName: {ProductName}, Volume: {Volume}, Price: {Price}, ProductType: {ProductType}, ProductSelection: {ProductSelection}, StoreCategory: {StoreCategory}, Fylde: {Fylde}, Freshness: {Freshness}, Garvestoffer: {Garvestoffer}, Bitterness: {Bitterness}, Sweetness: {Sweetness}, Color: {Color}, Smell: {Smell}, Taste: {Taste}, FitsWith1: {FitsWith1}, FitsWith2: {FitsWith2}, FitsWith3: {FitsWith3}, Country: {Country}, District: {District}, Subdistrict: {Subdistrict}, Year: {Year}, RawMaterial: {RawMaterial}, Method: {Method}, Alcohol: {Alcohol}, Sugar: {Sugar}, Acid: {Acid}, Lagringsgrad: {Lagringsgrad}, Producer: {Producer}, Grossist: {Grossist}, Distributor: {Distributor}, Emballasjetype: {Emballasjetype}, Korktype: {Korktype}";
        }
    }
}
