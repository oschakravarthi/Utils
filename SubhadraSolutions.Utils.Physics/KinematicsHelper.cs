namespace SubhadraSolutions.Utils.Physics
{
    public static class KinematicsHelper
    {
        public static double CalculateDisplacement(double u, double v, double t)
        {
            var a = v - u;
            var s = (u * t) + ((a * t * t) / 2);
            return s;
        }
        public static double CalculateTime(double u, double v, double remainingDisplacement)
        {
            var a = v - u;
            var t = Math.Sqrt(2 * remainingDisplacement / a);
            return t;
        }
    }
}
