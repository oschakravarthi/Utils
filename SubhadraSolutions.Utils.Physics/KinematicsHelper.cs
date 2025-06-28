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
    }
}
