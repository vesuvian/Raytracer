using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace Raytracer.Utils
{
	public static class ColorUtils
	{
		private const float EPSILON = 0.00001f;

		public static Color Lerp(Color a, Color b, float f)
		{
			Vector4 hslA = RgbToHsl(a);
			Vector4 hslB = RgbToHsl(b);
			Vector4 hslLerped = Vector4.Lerp(hslA, hslB, f);

			return HslToRgb(hslLerped);
		}

        /// <summary>
        /// Converts RGB to HSL.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        public static Vector4 RgbToHsl(Color color)
        {
	        // normalize red, green, blue values
            float r = color.R / 255.0f;
            float g = color.G / 255.0f;
            float b = color.B / 255.0f;
            float a = color.A / 255.0f;

            float max = System.Math.Max(r, System.Math.Max(g, b));
            float min = System.Math.Min(r, System.Math.Min(g, b));

            // hue
            float h = 0;
            if (System.Math.Abs(max - min) < EPSILON)
                h = 0; // undefined
            else if (System.Math.Abs(max - r) < EPSILON && g >= b)
                h = 60 * (g - b) / (max - min);
            else if (System.Math.Abs(max - r) < EPSILON && g < b)
                h = 60 * (g - b) / (max - min) + 360;
            else if (System.Math.Abs(max - g) < EPSILON)
                h = 60 * (b - r) / (max - min) + 120;
            else if (System.Math.Abs(max - b) < EPSILON)
                h = 60 * (r - g) / (max - min) + 240;

            // luminance
            float l = (max + min) / 2;

            // saturation
            float s = 0;
            if (System.Math.Abs(l) < EPSILON || System.Math.Abs(max - min) < EPSILON)
                s = 0;
            else if (0 < l && l <= 0.5f)
                s = (max - min) / (max + min);
            else if (l > 0.5f)
                s = (max - min) / (2 - (max + min));

            return new Vector4(MathUtils.Clamp(h, 0, 360),
                               MathUtils.Clamp(s, 0, 1),
                               MathUtils.Clamp(l, 0, 1),
                               MathUtils.Clamp(a, 0, 1));
        }
        /// <summary>
        /// Converts HSL to RGB.
        /// </summary>
        /// <param name="hsla"></param>
        public static Color HslToRgb(Vector4 hsla)
        {
	        float h = MathUtils.Clamp(hsla.X, 0, 360);
            float s = MathUtils.Clamp(hsla.Y, 0, 1);
            float l = MathUtils.Clamp(hsla.Z, 0, 1);
            float a = MathUtils.Clamp(hsla.W, 0, 1);

            // achromatic argb (gray scale)
            if (System.Math.Abs(s) < EPSILON)
            {
                return Color.FromArgb((int)MathUtils.Clamp(a * 255, 0, 255),
                                      (int)MathUtils.Clamp(l * 255, 0, 255),
                                      (int)MathUtils.Clamp(l * 255, 0, 255),
                                      (int)MathUtils.Clamp(l * 255, 0, 255));
            }

            float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
            float p = 2 * l - q;
            float hk = h / 360;

            float[] rgb =
            {
	            hk + 1 / 3.0f,
	            hk,
	            hk - 1 / 3.0f
            };

            for (int i = 0; i < 3; i++)
            {
                if (rgb[i] < 0)
                    rgb[i] += 1;
                if (rgb[i] > 1)
                    rgb[i] -= 1;
                if (rgb[i] * 6 < 1)
                    rgb[i] = p + (q - p) * 6 * rgb[i];
                else if (rgb[i] * 2 < 1)
                    rgb[i] = q;
                else if (rgb[i] * 3 < 2)
                    rgb[i] = p + (q - p) * (2 / 3.0f - rgb[i]) * 6;
                else
                    rgb[i] = p;
            }

            return Color.FromArgb((int)MathUtils.Clamp(a * 255, 0, 255),
                                  (int)MathUtils.Clamp(rgb[0] * 255, 0, 255),
                                  (int)MathUtils.Clamp(rgb[1] * 255, 0, 255),
                                  (int)MathUtils.Clamp(rgb[2] * 255, 0, 255));
        }

        public static Color Average(IEnumerable<Color> colors)
        {
	        int count = 0;
	        int r = 0;
	        int g = 0;
	        int b = 0;
	        int a = 0;

	        foreach (Color color in colors)
	        {
		        count++;
		        r += color.R;
		        g += color.G;
		        b += color.B;
		        a += color.A;
	        }

	        return Color.FromArgb((int)(a / (float)count),
	                              (int)(r / (float)count),
	                              (int)(g / (float)count),
	                              (int)(b / (float)count));
        }
	}
}
