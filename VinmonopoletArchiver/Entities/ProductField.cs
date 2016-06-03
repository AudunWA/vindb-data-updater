using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinmonopoletArchiver.Entities
{
    internal enum ProductField
    {
        ID, // varenummer
        TimeAcquired, // tid_hentet
        ProductName, // varenavn
        Volume, // volum
        Price, // pris
        ProductType, // varetype
        ProductSelection, // produktutvalg
        StoreCategory, // butikkategori
        Fylde, // fylde TODO: English
        Freshness, // friskhet
        Garvestoffer, // garvestoffer TODO: English
        Bitterness, // bitterhet
        Sweetness, // sodme
        Color, // farge
        Smell, // lukt
        Taste, // smak
        FitsWith1, // passer_til_1
        FitsWith2, // passer_til_2
        FitsWith3, // passer_til_3
        Country, // land
        District, // district
        Subdistrict, // underdistrikt
        Year, // argang
        RawMaterial, // rastoff
        Method, // metode
        Alcohol, // alkohol
        Sugar, // sukker
        Acid, // syre
        Lagringsgrad, // lagringsgrad TODO: English
        Producer, // produsent
        Grossist, // grossist TODO?: English
        Distributor, // distributor
        Emballasjetype, // emballasjetype TODO: English
        Korktype // korktype TODO: English
    }
}
