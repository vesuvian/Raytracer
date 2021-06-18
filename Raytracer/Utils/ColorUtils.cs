using System;
using System.Drawing;
using System.Numerics;

namespace Raytracer.Utils
{
	public class ColorUtils
	{
		private const float Epsilon = 0.00001f;

		public static Color Lerp(Color a, Color b, float f)
		{
			var hslA = RgBtoHsl(a);
			var hslB = RgBtoHsl(b);
			Vector3 hslLerped = Vector3.Lerp(hslA, hslB, f);
			float alphaLerped = MathUtils.Lerp(a.A, b.A, f);

			return HsLtoRgb(hslLerped.X, hslLerped.Y, hslLerped.Z, alphaLerped);
		}

        /// <summary>
        /// Converts RGB to HSL. Alpha is ignored.
        /// Output is: { H: [0, 360], S: [0, 1], L: [0, 1] }.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        public static Vector3 RgBtoHsl(Color color)
        {
            double h = 0D;
            double s = 0D;
            double l;
            // normalize red, green, blue values
            double r = color.R / 255D;
            double g = color.G / 255D;
            double b = color.B / 255D;
            double max = System.Math.Max(r, System.Math.Max(g, b));
            double min = System.Math.Min(r, System.Math.Min(g, b));
            // hue
            if (System.Math.Abs(max - min) < Epsilon)
                h = 0D; // undefined
            else if ((System.Math.Abs(max - r) < Epsilon)
                    && (g >= b))
                h = (60D * (g - b)) / (max - min);
            else if ((System.Math.Abs(max - r) < Epsilon)
                    && (g < b))
                h = ((60D * (g - b)) / (max - min)) + 360D;
            else if (System.Math.Abs(max - g) < Epsilon)
                h = ((60D * (b - r)) / (max - min)) + 120D;
            else if (System.Math.Abs(max - b) < Epsilon)
                h = ((60D * (r - g)) / (max - min)) + 240D;
            // luminance
            l = (max + min) / 2D;
            // saturation
            if ((System.Math.Abs(l) < Epsilon)
                    || (System.Math.Abs(max - min) < Epsilon))
                s = 0D;
            else if ((0D < l)
                    && (l <= .5D))
                s = (max - min) / (max + min);
            else if (l > .5D)
                s = (max - min) / (2D - (max + min)); //(max-min > 0)?
            return new Vector3
            (
                MathF.Max(0f, MathF.Min(360f, float.Parse($"{h:0.##}"))),
                MathF.Max(0f, MathF.Min(1f, float.Parse($"{s:0.##}"))),
                MathF.Max(0f, MathF.Min(1f, float.Parse($"{l:0.##}")))
            );
        }
        /// <summary>
        /// Converts HSL to RGB, with a specified output Alpha.
        /// Arguments are limited to the defined range:
        /// does not raise exceptions.
        /// </summary>
        /// <param name="h">Hue, must be in [0, 360].</param>
        /// <param name="s">Saturation, must be in [0, 1].</param>
        /// <param name="l">Luminance, must be in [0, 1].</param>
        /// <param name="a">Output Alpha, must be in [0, 1].</param>
        public static Color HsLtoRgb(double h, double s, double l, double a = 1)
        {
            h = System.Math.Max(0D, System.Math.Min(360D, h));
            s = System.Math.Max(0D, System.Math.Min(1D, s));
            l = System.Math.Max(0D, System.Math.Min(1D, l));
            a = System.Math.Max(0D, System.Math.Min(1D, a));
            // achromatic argb (gray scale)
            if (System.Math.Abs(s) < Epsilon)
            {
                return Color.FromArgb(
                        System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{a * 255D:0.00}")))),
                        System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))));
            }
            double q = l < .5D
                    ? l * (1D + s)
                    : (l + s) - (l * s);
            double p = (2D * l) - q;
            double hk = h / 360D;
            double[] T = new double[3];
            T[0] = hk + (1D / 3D); // Tr
            T[1] = hk; // Tb
            T[2] = hk - (1D / 3D); // Tg
            for (int i = 0; i < 3; i++)
            {
                if (T[i] < 0D)
                    T[i] += 1D;
                if (T[i] > 1D)
                    T[i] -= 1D;
                if ((T[i] * 6D) < 1D)
                    T[i] = p + ((q - p) * 6D * T[i]);
                else if ((T[i] * 2D) < 1)
                    T[i] = q;
                else if ((T[i] * 3D) < 2)
                    T[i] = p + ((q - p) * ((2D / 3D) - T[i]) * 6D);
                else
                    T[i] = p;
            }
            return Color.FromArgb(
                    System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{a * 255D:0.00}")))),
                    System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{T[0] * 255D:0.00}")))),
                    System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{T[1] * 255D:0.00}")))),
                    System.Math.Max(0, System.Math.Min(255, Convert.ToInt32(double.Parse($"{T[2] * 255D:0.00}")))));
        }
    }
}
