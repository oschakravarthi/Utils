//using System;

//namespace SubhadraSolutions.Utils.Mathematics;

//public class SexagesimalAngle
//{
//    public int Degrees { get; set; }
//    public bool IsNegative { get; set; }
//    public int Milliseconds { get; set; }
//    public int Minutes { get; set; }
//    public int Seconds { get; set; }

//    public static SexagesimalAngle FromDouble(double angleInDegrees)
//    {
//        //ensure the value will fall within the primary range [-180.0..+180.0]
//        while (angleInDegrees < -180.0)
//            angleInDegrees += 360.0;

//        while (angleInDegrees > 180.0)
//            angleInDegrees -= 360.0;

//        var result = new SexagesimalAngle
//        {
//            //switch the value to positive
//            IsNegative = angleInDegrees < 0
//        };

//        angleInDegrees = Math.Abs(angleInDegrees);

//        //gets the degree
//        result.Degrees = (int)Math.Floor(angleInDegrees);
//        var delta = angleInDegrees - result.Degrees;

//        //gets minutes and seconds
//        var seconds = (int)Math.Floor(3600.0 * delta);
//        result.Seconds = seconds % 60;
//        result.Minutes = (int)Math.Floor(seconds / 60.0);
//        delta = delta * 3600.0 - seconds;

//        //gets fractions
//        result.Milliseconds = (int)(1000.0 * delta);

//        return result;
//    }

//    public override string ToString()
//    {
//        var degrees = IsNegative
//            ? -Degrees
//            : Degrees;

//        return $"{degrees}° {Minutes:00}' {Seconds:00}\"";
//    }

//    public string ToString(string format)
//    {
//        switch (format)
//        {
//            case "NS":
//                return $"{Degrees}° {Minutes:00}' {Seconds:00}\".{Milliseconds:000} {(IsNegative ? 'S' : 'N')}";

//            case "WE":
//                return $"{Degrees}° {Minutes:00}' {Seconds:00}\".{Milliseconds:000} {(IsNegative ? 'W' : 'E')}";

//            default:
//                throw new NotImplementedException();
//        }
//    }
//}