#region USINGS

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Expanse;
using Expanse.Misc;
using Expanse.Motion;
using Expanse.Utilities;
using Expanse.Extensions;

#endregion

namespace Expanse.Utilities
{
    // References:
    // https://www.codeproject.com/Articles/19045/Manipulating-colors-in-NET-Part
    // https://github.com/THEjoezack/ColorMine
    // http://www.deathbysoftware.com/colors/index.html

    public static class ColorSpace
    {
        public interface IColorSpace
        {
            RGB ToRGB();
            void FromRGB(RGB rgb);
        }

        public struct RGB : IColorSpace
        {
            public double r;
            public double g;
            public double b;

            public RGB(double r, double g, double b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }
            
            public RGB(Color color)
            {
                this.r = color.r;
                this.g = color.g;
                this.b = color.b;
            }

            public RGB(Color32 color)
            {
                this.r = color.r / 255.0;
                this.g = color.g / 255.0;
                this.b = color.b / 255.0;
            }

            public RGB ToRGB()
            {
                return this;
            }

            public void FromRGB(RGB rgb)
            {
                this = rgb;
            }

            public Color ToColor(float a = 1f)
            {
                return new Color((float)r, (float)g, (float)b, a);
            }

            public Color32 ToColor32(byte a = 0xFF)
            {
                return new Color32((byte)(0xFF / r), (byte)(0xFF / g), (byte)(0xFF / b), a);
            }
        }

        public struct RYB : IColorSpace
        {
            public double r;
            public double y;
            public double b;

            public RYB(double r, double y, double b)
            {
                this.r = r;
                this.y = y;
                this.b = b;
            }

            public RGB ToRGB()
            {
                return RYBConverter.ToRGB(this);
            }

            public void FromRGB(RGB rgb)
            {
                this = RYBConverter.FromRGB(rgb);
            }
        }

        public struct CMY : IColorSpace
        {
            public double c;
            public double m;
            public double y;

            public CMY(double c, double m, double y)
            {
                this.c = c;
                this.m = m;
                this.y = y;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct CMYK : IColorSpace
        {
            public double c;
            public double m;
            public double y;
            public double k;

            public CMYK(double c, double m, double y, double k)
            {
                this.c = c;
                this.m = m;
                this.y = y;
                this.k = k;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct HSL : IColorSpace
        {
            public double h;
            public double s;
            public double l;

            public HSL(double h, double s, double l)
            {
                this.h = h;
                this.s = s;
                this.l = l;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct HSB : IColorSpace
        {
            public double h;
            public double s;
            public double b;

            public HSB(double h, double s, double b)
            {
                this.h = h;
                this.s = s;
                this.b = b;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct HSV : IColorSpace
        {
            public double h;
            public double s;
            public double v;

            public HSV(double h, double s, double v)
            {
                this.h = h;
                this.s = s;
                this.v = v;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct CIE_LAB : IColorSpace
        {
            public double l;
            public double a;
            public double b;

            public CIE_LAB(double l, double a, double b)
            {
                this.l = l;
                this.a = a;
                this.b = b;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct Hunter_LAB : IColorSpace
        {
            public double l;
            public double a;
            public double b;

            public Hunter_LAB(double l, double a, double b)
            {
                this.l = l;
                this.a = a;
                this.b = b;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct LHC : IColorSpace
        {
            public double l;
            public double h;
            public double c;

            public LHC(double l, double h, double c)
            {
                this.l = l;
                this.h = h;
                this.c = c;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct LUV : IColorSpace
        {
            public double l;
            public double u;
            public double v;

            public LUV(double l, double u, double v)
            {
                this.l = l;
                this.u = u;
                this.v = v;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct XYZ : IColorSpace
        {
            public double x;
            public double y;
            public double z;

            public XYZ(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public RGB ToRGB()
            {
                return XYZConverter.ToRGB(this);
            }

            public void FromRGB(RGB rgb)
            {
                this = XYZConverter.FromRGB(rgb);
            }
        }

        public struct YXY : IColorSpace
        {
            public double y1;
            public double x;
            public double y2;

            public YXY(double y1, double x, double y2)
            {
                this.y1 = y1;
                this.x = x;
                this.y2 = y2;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }

        public struct YUV : IColorSpace
        {
            public double y;
            public double u;
            public double v;

            public YUV(double y, double u, double v)
            {
                this.y = y;
                this.u = u;
                this.v = v;
            }

            public RGB ToRGB()
            {
                throw new NotImplementedException();
            }

            public void FromRGB(RGB rgb)
            {
                throw new NotImplementedException();
            }
        }
    }
}
