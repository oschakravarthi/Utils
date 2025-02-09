using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;
using SubhadraSolutions.Utils.Astronomy.Planets.MeanOrbitalElements;
using SubhadraSolutions.Utils.Astronomy.Planets.SphericalLBRCoordinates;

namespace SubhadraSolutions.Utils.Astronomy.Planets
{
    public class Planet
    {
        private Moment _moment;

        public Planet(Moment moment)
        {
            _moment = moment;
            this.OrbitalElements = new OrbitalElements();
            this.SphericalCoordinates = new SphericalCoordinates();
        }

        public Moment Moment
        {
            get
            {
                return _moment;
            }
            set
            {
                _moment = value;
                this.OrbitalElements = new OrbitalElements();
                this.SphericalCoordinates = new SphericalCoordinates();
            }
        }

        public OrbitalElements OrbitalElements { get; set; }

        public SphericalCoordinates SphericalCoordinates { get; set; }
    }
}
