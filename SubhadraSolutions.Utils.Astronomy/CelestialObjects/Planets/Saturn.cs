using SubhadraSolutions.Utils.Astronomy.Planets.MeanOrbitalElements;
using SubhadraSolutions.Utils.Astronomy.Planets.SphericalLBRCoordinates;
using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;

namespace SubhadraSolutions.Utils.Astronomy.Planets
{
    public class Saturn : Planet
    {
        public Saturn(Moment moment) : base(moment) 
        {
            OrbitalElements = OrbitalElementsBuilder.Create(this);
            SphericalCoordinates = SphericalCoordinatesBuilder.Create(this);
        }

    }
}
