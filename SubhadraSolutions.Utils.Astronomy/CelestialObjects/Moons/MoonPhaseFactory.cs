using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;
using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Astronomy.CelestialObjects.Moons
{
    public static class MoonPhaseFactory
    {
        public static MoonPhase Create(string phaseName, Moment moment)
        {
            string[] phases = { PhaseName.NewMoon, PhaseName.FirstQuarter, PhaseName.FullMoon, PhaseName.LastQuarter };
            if (!phases.Contains(phaseName))
            {
                throw new ArgumentOutOfRangeException(nameof(phaseName), $"Unexpected phase name '{phaseName}'");
            }

            return new MoonPhase(moment, phaseName);
        }
    }
}
