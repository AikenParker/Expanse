using System;
using static Expanse.Utilities.ColorSpace;

namespace Expanse.Utilities
{
    public static class XYZConverter
    {
        public static RGB ToRGB(XYZ xyz)
        {
            double r = xyz.x * 3.2410 - xyz.y * 1.5374 - xyz.z * 0.4986;
            double g = -xyz.x * 0.9692 + xyz.y * 1.8760 - xyz.z * 0.0416;
            double b = xyz.x * 0.0556 - xyz.y * 0.2040 + xyz.z * 1.0570;

            RGB rgb = new RGB();

            return rgb;
        }

        public static XYZ FromRGB(RGB rgb)
        {
            double r = (rgb.r > 0.04045) ? Math.Pow((rgb.r + 0.055) / (1 + 0.055), 2.2)
                : (rgb.r / 12.92);
            double g = (rgb.g > 0.04045) ? Math.Pow((rgb.g + 0.055) / (1 + 0.055), 2.2)
                : (rgb.g / 12.92);
            double b = (rgb.b > 0.04045) ? Math.Pow((rgb.b + 0.055) / (1 + 0.055), 2.2)
                : (rgb.b / 12.92);

            XYZ xyz = new XYZ
            {
                x = rgb.r * 0.4124 + rgb.g * 0.3576 + rgb.b * 0.1805,
                y = rgb.r * 0.2126 + rgb.g * 0.7152 + rgb.b * 0.0722,
                z = rgb.r * 0.0193 + rgb.g * 0.1192 + rgb.b * 0.9505
            };

            return xyz;
        }
    }

    public static class RYBConverter
    {
        public static RGB ToRGB(RYB ryb)
        {
            double r = ryb.r * 255;
            double y = ryb.y * 255;
            double b = ryb.b * 255;

            double white = Math.Min(Math.Min(r, y), b);

            r -= white;
            y -= white;
            b -= white;

            double maxY = Math.Max(Math.Max(r, y), b);
            double g = Math.Min(y, b);

            y -= g;
            b -= g;

            if (b > 0 && g > 0)
            {
                b *= 2.0;
                g *= 2.0;
            }

            r += y;
            g += y;

            double maxG = Math.Max(Math.Max(r, g), b);

            if (maxG > 0)
            {
                double factor = maxY / maxG;

                r *= factor;
                g *= factor;
                b *= factor;
            }

            r += white;
            g += white;
            b += white;

            RGB rgb = new RGB
            {
                r = r / 255,
                g = g / 255,
                b = b / 255
            };

            return rgb;
        }

        public static RYB FromRGB(RGB rgb)
        {
            double r = rgb.r * 255;
            double g = rgb.g * 255;
            double b = rgb.b * 255;

            double white = Math.Min(Math.Min(r, g), b);

            r -= white;
            g -= white;
            b -= white;

            double maxG = Math.Max(Math.Max(r, g), b);
            double y = Math.Min(r, g);

            r -= y;
            g -= y;

            if (b > 0 && g > 0)
            {
                b *= 0.5;
                g *= 0.5;
            }

            y += g;
            b += g;

            double maxY = Math.Max(Math.Max(r, y), b);

            if (maxY > 0)
            {
                double factor = maxG / maxY;

                r *= factor;
                y *= factor;
                b *= factor;
            }

            r += white;
            y += white;
            b += white;

            RYB ryb = new RYB
            {
                r = r / 255,
                y = y / 255,
                b = b / 255
            };

            return ryb;
        }
    }
}
