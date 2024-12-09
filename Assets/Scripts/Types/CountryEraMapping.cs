using System.Collections.Generic;

public class CountryEraMapping
{
    public Dictionary<Countries, List<Eras>> CountryToEras = new Dictionary<Countries, List<Eras>>
    {
        { Countries.Greece, new List<Eras> { Eras.StoneAge, Eras.NeolithicAge } },
        // { Countries.France, new List<Eras> { Eras.MedievalAge } },
        // ... add mappings for other countries
    };
}