using SubhadraSolutions.Utils.Astronomy.Planets.MeanOrbitalElements;
using SubhadraSolutions.Utils.Astronomy.Planets.SphericalLBRCoordinates;
using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;

namespace SubhadraSolutions.Utils.Astronomy.Planets
{
    public class Mercury : Planet
    {
        public Mercury(Moment moment) : base(moment) 
        {
            OrbitalElements = OrbitalElementsBuilder.Create(this);
            SphericalCoordinates = SphericalCoordinatesBuilder.Create(this);
        }
    }
}
