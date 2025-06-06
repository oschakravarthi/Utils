﻿using SubhadraSolutions.Utils.Astronomy.UnitsOfMeasure;
using System;

namespace SubhadraSolutions.Utils.Astronomy.Models
{

	/// <summary>
	/// Sundial hour angle for a given latitude and hour.
	/// </summary>
	public struct SundialAngle
	{
		public int Hour { get; private set; }
		public Degrees Angle { get; private set; }

		public SundialAngle(int hour, Degrees angle)
		{
			Hour = hour;
			Angle = angle;
		}

		public override string ToString()
		{
			int h = (Hour > 12) ? (Hour - 12) : Hour;
			string period = GetHourPeriod(Hour);
			double truncatedAngle = Math.Truncate(Angle * 100) / 100;
			string angle = string.Format("{0:N2}", truncatedAngle);
            return $"{h} {period}: {angle}";
		}

        private string GetHourPeriod(int h)
        {
            return h switch
            {
                > 12 => "PM",
                < 12 => "AM",
                _ => "Solar Noon"
            };
        }
    }
}
