using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;

namespace SubhadraSolutions.Utils.Astronomy.CelestialObjects.Moons
{
    public class MoonPhase
    {
        private Moment _moment;
        private string _phaseName;

        public MoonPhase(Moment moment, string phaseName)
        {
            _moment = moment;
            _phaseName = phaseName;
        }

        public Moment Moment { get { return _moment; } }
        public string PhaseName { get { return _phaseName; } }
    }
}