using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinmonopoletArchiver.Entities
{
    public class Product
    {
        public long ID { get; set; } // varenummer
        public DateTime FirstSeen { get; set; } // first_seen
        public DateTime LastSeen { get; set; } // last_seen
        public string ProductName { get; set; } // varenavn
        public double Volume { get; set; } // volum
        public double Price { get; set; } // pris
        public string ProductType { get; set; } // varetype
        public string ProductSelection { get; set; } // produktutvalg
        public string StoreCategory { get; set; } // butikkategori
        public byte Fylde { get; set; } // fylde TODO: English (Fullness?)
        public byte Freshness { get; set; } // friskhet
        public byte Garvestoffer { get; set; } // garvestoffer TODO: English (Tanins?)
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
        public int? Year { get; set; } // argang
        public string RawMaterial { get; set; } // rastoff
        public string Method { get; set; } // metode
        public double Alcohol { get; set; } // alkohol
        public double? Sugar { get; set; } // sukker
        public double? Acid { get; set; } // syre
        public string Lagringsgrad { get; set; } // lagringsgrad TODO: English (StorageType?)
        public string Producer { get; set; } // produsent
        public string Grossist { get; set; } // grossist TODO?: English (Wholesaler?)
        public string Distributor { get; set; } // distributor
        public string Emballasjetype { get; set; } // emballasjetype TODO: English (PackagingType?)
        public string Korktype { get; set; } // korktype TODO: English (CorkType?)

        public DateTime TimeAcquired { get; set; } // Not used directly in DB


        public override string ToString()
        {
            return $"ID: {ID}, FirstSeen: {FirstSeen}, LastSeen: {LastSeen}, ProductName: {ProductName}, Volume: {Volume}, Price: {Price}, ProductType: {ProductType}, ProductSelection: {ProductSelection}, StoreCategory: {StoreCategory}, Fylde: {Fylde}, Freshness: {Freshness}, Garvestoffer: {Garvestoffer}, Bitterness: {Bitterness}, Sweetness: {Sweetness}, Color: {Color}, Smell: {Smell}, Taste: {Taste}, FitsWith1: {FitsWith1}, FitsWith2: {FitsWith2}, FitsWith3: {FitsWith3}, Country: {Country}, District: {District}, Subdistrict: {Subdistrict}, Year: {Year}, RawMaterial: {RawMaterial}, Method: {Method}, Alcohol: {Alcohol}, Sugar: {Sugar}, Acid: {Acid}, Lagringsgrad: {Lagringsgrad}, Producer: {Producer}, Grossist: {Grossist}, Distributor: {Distributor}, Emballasjetype: {Emballasjetype}, Korktype: {Korktype}";
        }

        public List<ProductChange> FindChanges(Product otherVersion)
        {
            List<ProductChange> changes = new List<ProductChange>();
            if (!Equals(ProductName, otherVersion.ProductName))
                changes.Add(new ProductChange(ID, ProductField.ProductName, otherVersion.LastSeen, ProductName, otherVersion.ProductName));

            if (!Equals(Volume, otherVersion.Volume))
                changes.Add(new ProductChange(ID, ProductField.Volume, otherVersion.LastSeen, Volume.ToString(), otherVersion.Volume.ToString()));

            if (!Equals(Price, otherVersion.Price))
                changes.Add(new ProductChange(ID, ProductField.Price, otherVersion.LastSeen, Price.ToString(), otherVersion.Price.ToString()));

            if (!Equals(ProductType, otherVersion.ProductType))
                changes.Add(new ProductChange(ID, ProductField.ProductType, otherVersion.LastSeen, ProductType, otherVersion.ProductType));

            if (!Equals(ProductSelection, otherVersion.ProductSelection))
                changes.Add(new ProductChange(ID, ProductField.ProductSelection, otherVersion.LastSeen, ProductSelection, otherVersion.ProductSelection));

            if (!Equals(StoreCategory, otherVersion.StoreCategory))
                changes.Add(new ProductChange(ID, ProductField.StoreCategory, otherVersion.LastSeen, StoreCategory, otherVersion.StoreCategory));

            if (!Equals(Fylde, otherVersion.Fylde))
                changes.Add(new ProductChange(ID, ProductField.Fylde, otherVersion.LastSeen, Fylde.ToString(), otherVersion.Fylde.ToString()));

            if (!Equals(Freshness, otherVersion.Freshness))
                changes.Add(new ProductChange(ID, ProductField.Freshness, otherVersion.LastSeen, Freshness.ToString(), otherVersion.Freshness.ToString()));

            if (!Equals(Garvestoffer, otherVersion.Garvestoffer))
                changes.Add(new ProductChange(ID, ProductField.Garvestoffer, otherVersion.LastSeen, Garvestoffer.ToString(), otherVersion.Garvestoffer.ToString()));

            if (!Equals(Bitterness, otherVersion.Bitterness))
                changes.Add(new ProductChange(ID, ProductField.Bitterness, otherVersion.LastSeen, Bitterness.ToString(), otherVersion.Bitterness.ToString()));

            if (!Equals(Sweetness, otherVersion.Sweetness))
                changes.Add(new ProductChange(ID, ProductField.Sweetness, otherVersion.LastSeen, Sweetness.ToString(), otherVersion.Sweetness.ToString()));

            if (!Equals(Color, otherVersion.Color))
                changes.Add(new ProductChange(ID, ProductField.Color, otherVersion.LastSeen, Color, otherVersion.Color));

            if (!Equals(Smell, otherVersion.Smell))
                changes.Add(new ProductChange(ID, ProductField.Smell, otherVersion.LastSeen, Smell, otherVersion.Smell));

            if (!Equals(Taste, otherVersion.Taste))
                changes.Add(new ProductChange(ID, ProductField.Taste, otherVersion.LastSeen, Taste, otherVersion.Taste));

            if (!Equals(FitsWith1,otherVersion.FitsWith1))
                changes.Add(new ProductChange(ID, ProductField.FitsWith1, otherVersion.LastSeen, FitsWith1, otherVersion.FitsWith1));

            if (!Equals(FitsWith2, otherVersion.FitsWith2))
                changes.Add(new ProductChange(ID, ProductField.FitsWith2, otherVersion.LastSeen, FitsWith2, otherVersion.FitsWith2));

            if (!Equals(FitsWith3, otherVersion.FitsWith3))
                changes.Add(new ProductChange(ID, ProductField.FitsWith3, otherVersion.LastSeen, FitsWith3, otherVersion.FitsWith3));

            if (!Equals(Country, otherVersion.Country))
                changes.Add(new ProductChange(ID, ProductField.Country, otherVersion.LastSeen, Country, otherVersion.Country));

            if (!Equals(District, otherVersion.District))
                changes.Add(new ProductChange(ID, ProductField.District, otherVersion.LastSeen, District, otherVersion.District));

            if (!Equals(Subdistrict, otherVersion.Subdistrict))
                changes.Add(new ProductChange(ID, ProductField.Subdistrict, otherVersion.LastSeen, Subdistrict, otherVersion.Subdistrict));

            if (!Equals(Year, otherVersion.Year))
                changes.Add(new ProductChange(ID, ProductField.Year, otherVersion.LastSeen, Year.ToString(), otherVersion.Year.ToString()));

            if (!Equals(RawMaterial, otherVersion.RawMaterial))
                changes.Add(new ProductChange(ID, ProductField.RawMaterial, otherVersion.LastSeen, RawMaterial, otherVersion.RawMaterial));

            if (!Equals(Method, otherVersion.Method))
                changes.Add(new ProductChange(ID, ProductField.Method, otherVersion.LastSeen, Method, otherVersion.Method));

            if (!Equals(Alcohol, otherVersion.Alcohol))
                changes.Add(new ProductChange(ID, ProductField.Alcohol, otherVersion.LastSeen, Alcohol.ToString(), otherVersion.Alcohol.ToString()));

            if (!Equals(Sugar, otherVersion.Sugar))
                changes.Add(new ProductChange(ID, ProductField.Sugar, otherVersion.LastSeen, Sugar.ToString(), otherVersion.Sugar.ToString()));

            if (!Equals(Acid, otherVersion.Acid))
                changes.Add(new ProductChange(ID, ProductField.Acid, otherVersion.LastSeen, Acid.ToString(), otherVersion.Acid.ToString()));

            if (!Equals(Lagringsgrad, otherVersion.Lagringsgrad))
                changes.Add(new ProductChange(ID, ProductField.Lagringsgrad, otherVersion.LastSeen, Lagringsgrad, otherVersion.Lagringsgrad));

            if (!Equals(Producer, otherVersion.Producer))
                changes.Add(new ProductChange(ID, ProductField.Producer, otherVersion.LastSeen, Producer, otherVersion.Producer));

            if (!Equals(Grossist, otherVersion.Grossist))
                changes.Add(new ProductChange(ID, ProductField.Grossist, otherVersion.LastSeen, Grossist, otherVersion.Grossist));

            if (!Equals(Distributor, otherVersion.Distributor))
                changes.Add(new ProductChange(ID, ProductField.Distributor, otherVersion.LastSeen, Distributor, otherVersion.Distributor));

            if (!Equals(Emballasjetype, otherVersion.Emballasjetype))
                changes.Add(new ProductChange(ID, ProductField.Emballasjetype, otherVersion.LastSeen, Emballasjetype, otherVersion.Emballasjetype));

            if (!Equals(Korktype, otherVersion.Korktype))
                changes.Add(new ProductChange(ID, ProductField.Korktype, otherVersion.LastSeen, Korktype, otherVersion.Korktype));

            return changes;
        } 
    }
}
