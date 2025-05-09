﻿using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;

namespace SubhadraSolutions.Utils.Astronomy.Models
{
    public struct Nutation
    {
        /// <summary>
        /// Nutation in Longitude
        /// </summary>
        public SexigesimalAngle ΔΨ { get; set; }
        
        /// <summary>
        /// Nutation in Obliquity
        /// </summary>
        public SexigesimalAngle Δε { get; set; }
        
        /// <summary>
        /// Obliquity of the Ecliptic
        /// </summary>
        public SexigesimalAngle ε { get; set; }
    }
}
