using VectSharp;

namespace SubhadraSolutions.Utils.SVG
{
    public class ExtendedGraphics : Graphics
    {
        public double ScaleExtended { get; set; } = 1.0;

        public Rectangle GetScaledBounds()
        {
            var bounds = GetBounds();
            var size = new Size(bounds.Size.Width * ScaleExtended, bounds.Size.Width * ScaleExtended);
            return new Rectangle(bounds.Location, size);
        }
    }
}