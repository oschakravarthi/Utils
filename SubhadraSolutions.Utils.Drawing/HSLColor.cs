namespace SubhadraSolutions.Utils.Drawing;

public struct HSLColor(float hue, float saturation, float lightness)
{
    public float Hue { get; } = hue;
    public float Lightness { get; } = lightness;
    public float Saturation { get; } = saturation;
}