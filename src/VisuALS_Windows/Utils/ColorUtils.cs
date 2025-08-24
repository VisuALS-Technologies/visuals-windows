using System;
using System.Linq;
using System.Windows.Media;

namespace VisuALS_WPF_App
{
    public static class ColorUtils
    {
        public static Color FromHSV(int H, int S, int V)
        {
            int h = Clamp(H, 0, 360);
            int s = Clamp(S, 0, 100);
            int v = Clamp(V, 0, 100);
            int[] rgb = HSVtoRGB(h, s, v);
            return Color.FromRgb(Convert.ToByte(rgb[0]), Convert.ToByte(rgb[1]), Convert.ToByte(rgb[2]));
        }

        public static int[] HSV(this Color color)
        {
            return RGBtoHSV(color.R, color.G, color.B);
        }

        public static int[] RGBtoHSV(int R, int G, int B)
        {
            int r = Clamp(R, 0, 255);
            int g = Clamp(G, 0, 255);
            int b = Clamp(B, 0, 255);
            double[] rgb = new double[] { r / 255.0, g / 255.0, b / 255.0 };
            double Xmax = rgb.Max();
            double Xmin = rgb.Min();
            double v = Xmax;
            double c = Xmax - Xmin;
            double h = 0;
            double s = 0;

            if (c == 0)
            {
                h = 0;
            }
            else if (v == rgb[0])
            {
                h = 60 * (rgb[1] - rgb[2]) / c;
            }
            else if (v == rgb[1])
            {
                h = 120 + 60 * (rgb[2] - rgb[0]) / c;
            }
            else if (v == rgb[2])
            {
                h = 240 + 60 * (rgb[0] - rgb[1]) / c;
            }

            if (h < 0)
            {
                h = 360 + h;
            }

            if (h > 360)
            {
                h = h - 360;
            }

            if (v > 0)
            {
                s = c / v;
            }

            return new int[] { Convert.ToInt32(h), Convert.ToInt32(100 * s), Convert.ToInt32(100 * v) };
        }

        public static int[] HSVtoRGB(int H, int S, int V)
        {
            double[] i = HSVtoRGBintermediate(H, S, V);
            double m = V / 100.0 - V / 100.0 * S / 100.0;
            return new int[] { Convert.ToInt32(255 * (i[0] + m)), Convert.ToInt32(255 * (i[1] + m)), Convert.ToInt32(255 * (i[2] + m)) };
        }

        private static double[] HSVtoRGBintermediate(int H, int S, int V)
        {
            int h = Clamp(H, 0, 360);
            int s = Clamp(S, 0, 100);
            int v = Clamp(V, 0, 100);
            double C = v / 100.0 * s / 100.0;
            double Hprime = h / 60.0;
            double X = C * (1 - Math.Abs(Hprime % 2 - 1));
            if (0 <= Hprime && Hprime <= 1)
            {
                return new double[] { C, X, 0 };
            }
            else if (1 < Hprime && Hprime <= 2)
            {
                return new double[] { X, C, 0 };
            }
            else if (2 < Hprime && Hprime <= 3)
            {
                return new double[] { 0, C, X };
            }
            else if (3 < Hprime && Hprime <= 4)
            {
                return new double[] { 0, X, C };
            }
            else if (4 < Hprime && Hprime <= 5)
            {
                return new double[] { X, 0, C };
            }
            else if (5 < Hprime && Hprime <= 6)
            {
                return new double[] { C, 0, X };
            }
            else
            {
                return new double[] { 0, 0, 0 };
            }
        }

        public static int HuePerceptionFix(int Hue)
        {
            int h = Clamp(Hue, 0, 360);
            if (0 <= h && h <= 120)
            {
                return Convert.ToInt32(h * 2 / 3);
            }
            else if (120 < h && h <= 180)
            {
                return Convert.ToInt32((10 / 6.0) * h - 120);
            }
            else
            {
                return h;
            }
        }

        private static int Clamp(int val, int min, int max)
        {
            if (val > max)
            {
                return max;
            }
            else if (val < min)
            {
                return min;
            }
            else
            {
                return val;
            }
        }

        public static System.Drawing.Color WinMediaColorToDrawingColor(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Color DrawingColorToWinMediaColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static Color FromString(string hexColor)
        {
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(hexColor);
            return DrawingColorToWinMediaColor(c);
        }

        public static string ToString(Color color)
        {
            System.Drawing.Color c = WinMediaColorToDrawingColor(color);
            return System.Drawing.ColorTranslator.ToHtml(c);
        }
    }
}
