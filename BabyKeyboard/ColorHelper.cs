using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BabyKeyboard
{
    public static class ColorHelper
    {
        internal const byte MIN_B = 0;
        internal const byte MAX_B = 255;
        internal const short MIN_H = 0;
        internal const short MAX_H = 360;
        internal const byte MAX_S = 100;
        internal const byte MAX_L = 100;
        internal const byte MAX_V = 100;

        static Random rand = new System.Random(DateTime.Now.Second * DateTime.Now.Millisecond);
        
        public static Color RandomColor()
        {
            return Color.FromRgb((byte)rand.Next(255), (byte)rand.Next(255), (byte)rand.Next(255));
        }

        public static Color RandomColor(byte min)
        {
            return RandomColor(min, 255);
        }

        public static Color RandomColor(byte min, byte max)
        {
            min = Math.Max(MIN_B, min);
            int mm = Math.Min(MAX_B, max) - min;
            return Color.FromRgb((byte)(min + rand.Next(mm)), (byte)(min + rand.Next(mm)), (byte)(min + rand.Next(mm)));
        }

        public static Color RandomColor(short minH, short maxH, byte minS, byte maxS, byte minL, byte maxL)
        {
            var h = Math.Max(MIN_H, Math.Min(MAX_H, rand.Next(maxH - minH) + minH));
            var s = Math.Max(MIN_B, Math.Min(MAX_B, rand.Next(maxS - minS) + minS));
            var l = Math.Max(MIN_B, Math.Min(MAX_B, rand.Next(maxL - minL) + minL));
            return HslToRgb((short)h, (byte)s, (byte)l);
        }

        public static void RgbToHsl(byte red, byte green, byte blue, out short hue, out byte saturation, out byte lightness)
        {
            byte maxValue = Math.Max(red, Math.Max(green, blue));
            byte minValue = Math.Min(red, Math.Min(green, blue));
            int diffValue = maxValue - minValue;

            var h = 0;
            if (maxValue == minValue)
            {
                h = 0;
            }
            else
            {
                if (maxValue == red)
                {
                    if (green >= blue)
                        h = 60 * (green - blue) / diffValue;
                    else
                        h = 60 * (green - blue) / diffValue + 360;
                }
                else if (maxValue == green)
                {
                    h = 60 * (blue - red) / diffValue + 120;
                }
                else if (maxValue == blue)
                {
                    h = 60 * (red - green) / diffValue + 240;
                }
            }

            var l = (maxValue + minValue) * 100 / 255 / 2;

            int s;
            if (l == 0 || maxValue == minValue)
                s = 0;
            else if (l <= 50)
                s = diffValue * 100 / (maxValue + minValue);
            else
                s = diffValue * 100 / ((2 * 255) - (maxValue + minValue));

            hue = (short)h;
            saturation = (byte)s;
            lightness = (byte)l;
        }

        public static void HslToRgb(short hue, byte saturation, byte lightness, out byte red, out byte green, out byte blue)
        {
            double h = hue;
            double s = saturation / 100.0f;
            double l = lightness / 100.0f;

            if (s == 0.0)
            {
                red = (byte)(l * 255.0F);
                green = red;
                blue = red;
            }
            else
            {
                double rm1;
                double rm2;

                if (l <= 0.5f)
                {
                    rm2 = l + l * s;
                }
                else
                {
                    rm2 = l + s - l * s;
                }
                rm1 = 2.0f * l - rm2;
                red = ToRGB1(rm1, rm2, h + 120.0f);
                green = ToRGB1(rm1, rm2, h);
                blue = ToRGB1(rm1, rm2, h - 120.0f);
            }
        }

        static byte ToRGB1(double rm1, double rm2, double rh)
        {
            if (rh > 360.0f)
            {
                rh -= 360.0f;
            }
            else if (rh < 0.0f)
            {
                rh += 360.0f;
            }

            if (rh < 60.0f)
            {
                rm1 = rm1 + (rm2 - rm1) * rh / 60.0f;
            }
            else if (rh < 180.0f)
            {
                rm1 = rm2;
            }
            else if (rh < 240.0f)
            {
                rm1 = rm1 + (rm2 - rm1) * (240.0f - rh) / 60.0f;
            }

            return (byte)(rm1 * 255);
        }

        public static void HsvToRgb(short hue, byte saturation, byte value, out byte red, out byte green, out byte blue)
        {
            // ######################################################################
            // T. Nathan Mundhenk
            // mundhenk@usc.edu
            // C/C++ Macro HSV to RGB

            double h = hue;
            if (hue < 0)
                h = hue % 360 + 360;
            else
                h = hue % 360;
            saturation = Math.Max(MAX_S, Math.Min(MIN_B, saturation));
            value = Math.Max(MAX_V, Math.Min(MIN_B, value));

            double r, g, b;
            if (value <= 0)
            {
                r = g = b = 0;
            }
            else if (saturation <= 0)
            {
                r = g = b = value;
            }
            else
            {
                double v = value / 100.0;
                double s = saturation / 100.0;

                double hf = h / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = v * (1 - s);
                double qv = v * (1 - s * f);
                double tv = v * (1 - s * (1 - f));
                switch (i)
                {
                    // Red is the dominant color
                    case 0:
                        r = v;
                        g = tv;
                        b = pv;
                        break;
                    // Green is the dominant color
                    case 1:
                        r = qv;
                        g = v;
                        b = pv;
                        break;
                    case 2:
                        r = pv;
                        g = v;
                        b = tv;
                        break;
                    // Blue is the dominant color
                    case 3:
                        r = pv;
                        g = qv;
                        b = v;
                        break;
                    case 4:
                        r = tv;
                        g = pv;
                        b = v;
                        break;
                    // Red is the dominant color
                    case 5:
                        r = v;
                        g = pv;
                        b = qv;
                        break;
                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                        r = v;
                        g = tv;
                        b = pv;
                        break;
                    case -1:
                        r = v;
                        g = pv;
                        b = qv;
                        break;
                    // The color is not defined, we should throw an error.
                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        r = g = b = v; // Just pretend its black/white
                        break;
                }
            }

            red = Math.Min(MAX_B, Math.Max(MIN_B, (byte)(r * 255.0)));
            green = Math.Min(MAX_B, Math.Max(MIN_B, (byte)(g * 255.0)));
            blue = Math.Min(MAX_B, Math.Max(MIN_B, (byte)(b * 255.0)));
        }

        public static void RgbToHsv(byte red, byte green, byte blue, out short hue, out byte saturation, out byte value)
        {
            byte maxValue = Math.Max(red, Math.Max(green, blue));
            byte minValue = Math.Min(red, Math.Min(green, blue));

            var h = 0;
            if (maxValue == minValue)
            {
                h = 0;
            }
            else
            {
                if (maxValue == red)
                {
                    if (green >= blue)
                        h = 60 * (green - blue) / (maxValue - minValue);
                    else
                        h = 60 * (green - blue) / (maxValue - minValue) + 360;
                }
                else if (maxValue == green)
                {
                    h = 60 * (blue - red) / (maxValue - minValue) + 120;
                }
                else if (maxValue == blue)
                {
                    h = 60 * (red - green) / (maxValue - minValue) + 240;
                }
            }

            int s;
            if (maxValue == 0)
                s = 0;
            else
                s = (maxValue - minValue) * 100 / maxValue;

            var v = maxValue * 100 / 255;

            hue = (short)h;
            saturation = (byte)s;
            value = (byte)v;
        }

        public static Color HslToRgb(short h, byte s, byte l)
        {
            HslToRgb(h, s, l, out byte r, out byte g, out byte b);
            return Color.FromRgb(r, g, b);
        }

        public static Color HsvToRgb(short h, byte s, byte v)
        {
            HsvToRgb(h, s, v, out byte r, out byte g, out byte b);
            return Color.FromRgb(r, g, b);
        }

        public static Color GetBrighter(Color color1, Color color2)
        {
            color1.RgbToHsl(out short h, out byte s, out byte l1);
            color2.RgbToHsl(out h, out s, out byte l2);

            if (l1 > l2)
                return color1;
            else
                return color2;
        }

        public static Color GetDarker(Color color1, Color color2)
        {
            color1.RgbToHsl(out short h, out byte s, out byte l1);
            color2.RgbToHsl(out h, out s, out byte l2);

            if (l1 > l2)
                return color2;
            else
                return color1;
        }

        public static string ToString(Color color)
        {
            if (color.A < 255)
                return string.Format("#{0:X2}{1:X2}{2:X2}{2:X2}", color.A, color.R, color.G, color.B);
            else
                return string.Format("#{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
        }

        public static void RgbToHsl(this Color color, out short hue, out byte saturation, out byte lightness)
        {
            RgbToHsl(color.R, color.G, color.B, out hue, out saturation, out lightness);
        }

        public static void RgbToHsv(this Color rgbColor, out short hue, out byte saturation, out byte value)
        {
            RgbToHsv(rgbColor.R, rgbColor.G, rgbColor.B, out hue, out saturation, out value);
        }
    }
}
