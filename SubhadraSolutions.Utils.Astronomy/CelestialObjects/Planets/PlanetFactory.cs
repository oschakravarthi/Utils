using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;
using System;

namespace SubhadraSolutions.Utils.Astronomy.Planets
{
    public static class PlanetFactory
    {
        public static Planet Create(string planetName, Moment moment) => planetName switch
        {
            PlanetName.Mercury => new Mercury(moment),
            PlanetName.Venus => new Venus(moment),
            PlanetName.Earth => new Earth(moment),
            PlanetName.Mars => new Mars(moment),
            PlanetName.Jupiter => new Jupiter(moment),
            PlanetName.Saturn => new Saturn(moment),
            _ => throw new ArgumentOutOfRangeException(nameof(planetName), $"Unexpected planet name '{planetName}'")
        };
    }
}
