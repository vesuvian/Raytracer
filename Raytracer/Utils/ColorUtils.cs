using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Utils
{
	public static class ColorUtils
	{
		private const float EPSILON = 0.00001f;

		/// <summary>
		/// Gets the CIE D65 (white) structure.
		/// </summary>
		public static Vector3 XyzD65White = new Vector3(0.9505f, 1.0f, 1.0890f);

		public static Vector4 RgbaBlack { get { return new Vector4(0, 0, 0, 1); } }

		public static Vector4 RgbaWhite { get { return new Vector4(1); } }

		#region Lerp

		public static Color LerpRgb(Color a, Color b, float f)
		{
			Vector4 rgbA = ColorToRgb(a);
			Vector4 rgbB = ColorToRgb(b);
			Vector4 rgbLerped = LerpRgb(rgbA, rgbB, f);
			return RgbToColor(rgbLerped);
		}

		public static Vector4 LerpRgb(Vector4 a, Vector4 b, float f)
		{
			return Vector4.Lerp(a, b, f);
		}

		public static Color LerpHsl(Color a, Color b, float f)
		{
			Vector4 rgbaA = ColorToRgb(a);
			Vector4 rgbaB = ColorToRgb(b);
			Vector4 rgbLerped = LerpHsl(rgbaA, rgbaB, f);
			return RgbToColor(rgbLerped);
		}

		public static Vector4 LerpHsl(Vector4 rgbaA, Vector4 rgbaB, float f)
		{
			Vector4 hslA = RgbToHsl(rgbaA);
			Vector4 hslB = RgbToHsl(rgbaB);
			Vector4 hslLerped = Vector4.Lerp(hslA, hslB, f);
			return HslToRgb(hslLerped);
		}

		#endregion

		#region Conversion

		/// <summary>
		/// Converts RGB to HSL.
		/// </summary>
		public static Vector4 RgbToHsl(Vector4 rgba)
		{
			float r = rgba.X;
			float g = rgba.Y;
			float b = rgba.Z;
			float a = rgba.W;

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

			return new Vector4(MathUtils.ModPositive(h, 360),
			                   MathUtils.Clamp(s, 0, 1),
			                   MathUtils.Clamp(l, 0, 1),
			                   MathUtils.Clamp(a, 0, 1));
		}

		/// <summary>
		/// Converts HSL to RGB.
		/// </summary>
		/// <param name="hsla"></param>
		public static Vector4 HslToRgb(Vector4 hsla)
		{
			float h = MathUtils.ModPositive(hsla.X, 360);
			float s = MathUtils.Clamp(hsla.Y, 0, 1);
			float l = MathUtils.Clamp(hsla.Z, 0, 1);
			float a = MathUtils.Clamp(hsla.W, 0, 1);

			// achromatic argb (gray scale)
			if (System.Math.Abs(s) < EPSILON)
			{
				return new Vector4(MathUtils.Clamp(l, 0, 1),
				                   MathUtils.Clamp(l, 0, 1),
				                   MathUtils.Clamp(l, 0, 1),
				                   MathUtils.Clamp(a, 0, 1));
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

			return new Vector4(MathUtils.Clamp(rgb[0], 0, 1),
			                   MathUtils.Clamp(rgb[1], 0, 1),
			                   MathUtils.Clamp(rgb[2], 0, 1),
			                   MathUtils.Clamp(a, 0, 1));
		}

		public static Color RgbToColor(Vector4 rgba)
		{
			return Color.FromArgb((int)MathUtils.Clamp(rgba.W * 255, 0, 255),
			                      (int)MathUtils.Clamp(rgba.X * 255, 0, 255),
			                      (int)MathUtils.Clamp(rgba.Y * 255, 0, 255),
			                      (int)MathUtils.Clamp(rgba.Z * 255, 0, 255));
		}

		public static Vector4 ColorToRgb(Color color)
		{
			return new Vector4(color.R, color.G, color.B, color.A) / 255;
		}

		public static Vector4 RgbToLab(Vector4 rgba, Vector3 rgbWhite)
		{
			Vector4 xyz = RgbToXyz(rgba);
			Vector3 xyzWhite = RgbToXyz(rgbWhite.ToVector4()).ToVector3();

			return XyztoLab(xyz, xyzWhite);
		}

		public static Vector4 RgbToLab(Vector4 rgba)
		{
			Vector4 xyz = RgbToXyz(rgba);
			return XyztoLab(xyz, XyzD65White);
		}

		/// <summary>
		/// Converts RGB to CIEXYZ.
		/// </summary>
		public static Vector4 RgbToXyz(Vector4 rgba)
		{
			// convert to a sRGB form
			double r =
				rgba.X > 0.04045
					? System.Math.Pow((rgba.X + 0.055) / (1 + 0.055), 2.2)
					: rgba.X / 12.92;

			double g =
				rgba.Y > 0.04045
					? System.Math.Pow((rgba.Y + 0.055) / (1 + 0.055), 2.2)
					: rgba.Y / 12.92;

			double b =
				rgba.Z > 0.04045
					? System.Math.Pow((rgba.Z + 0.055) / (1 + 0.055), 2.2)
					: rgba.Z / 12.92;

			// converts
			return new Vector4((float)(r * 0.4124 + g * 0.3576 + b * 0.1805),
			                  (float)(r * 0.2126 + g * 0.7152 + b * 0.0722),
			                  (float)(r * 0.0193 + g * 0.1192 + b * 0.9505),
							  rgba.W);
		}

		/// <summary>
		/// Converts CIEXYZ to CIELab.
		/// </summary>
		public static Vector4 XyztoLab(Vector4 xyza, Vector3 xyzWhite)
		{
			// XYZ to L*a*b* transformation function.
			Func<double, double> fxyz = t =>
				((t > 0.008856)
					? System.Math.Pow(t, (1.0 / 3.0))
					: (7.787 * t + 16.0 / 116.0));

			return new Vector4((float)(116.0 * fxyz(xyza.Y / xyzWhite.Y) - 16),
			                   (float)(500.0 * (fxyz(xyza.X / xyzWhite.X) - fxyz(xyza.Y / xyzWhite.Y))),
			                   (float)(200.0 * (fxyz(xyza.Y / xyzWhite.Y) - fxyz(xyza.Z / xyzWhite.Z))),
			                   xyza.W);
		}

		#endregion

		#region Math

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

		public static Vector4 Average(IEnumerable<Vector4> samples)
		{
			int count = 0;
			Vector4 sum = default;

			foreach (Vector4 rgba in samples)
			{
				count++;
				sum += rgba;
			}

			return sum / count;
		}

		public static Color Sum(IEnumerable<Color> colors)
		{
			int r = 0;
			int g = 0;
			int b = 0;
			int a = 0;

			foreach (Color color in colors)
			{
				r += color.R;
				g += color.G;
				b += color.B;
				a += color.A;
			}

			return Color.FromArgb(System.Math.Min(a, 255),
			                      System.Math.Min(r, 255),
			                      System.Math.Min(g, 255),
			                      System.Math.Min(b, 255));
		}

		public static Vector4 Sum(IEnumerable<Vector4> colors)
		{
			return colors.Aggregate((a, b) => a + b);
		}

		public static Color Multiply(Color color, float scalar, bool alpha = false)
		{
			return Color.FromArgb(alpha ? MathUtils.Clamp((int)(color.A * scalar), 0, 255) : color.A,
			                      MathUtils.Clamp((int)(color.R * scalar), 0, 255),
			                      MathUtils.Clamp((int)(color.G * scalar), 0, 255),
			                      MathUtils.Clamp((int)(color.B * scalar), 0, 255));
		}

		public static Vector4 Multiply(Vector4 rgba, float scalar, bool alpha = false)
		{
			Vector4 output = rgba * scalar;
			if (!alpha)
				output.W = rgba.W;
			return output;
		}

		public static Color Multiply(Color a, Color b)
		{
			return Color.FromArgb((int)(a.A * b.A / 255.0f),
			                      (int)(a.R * b.R / 255.0f),
			                      (int)(a.G * b.G / 255.0f),
			                      (int)(a.B * b.B / 255.0f));
		}

		public static Vector4 Multiply(Vector4 a, Vector4 b)
		{
			return a * b;
		}

		public static Color Add(Color a, Color b)
		{
			return Color.FromArgb(System.Math.Min(a.A + b.A, 255),
			                      System.Math.Min(a.R + b.R, 255),
			                      System.Math.Min(a.G + b.G, 255),
			                      System.Math.Min(a.B + b.B, 255));
		}

		public static Vector4 Add(Vector4 a, Vector4 b)
		{
			return a + b;
		}

		public static Color Clamp(Color color, Color min, Color max)
		{
			return Color.FromArgb(MathUtils.Clamp(color.A, min.A, max.A),
			                      MathUtils.Clamp(color.R, min.R, max.R),
			                      MathUtils.Clamp(color.G, min.G, max.G),
			                      MathUtils.Clamp(color.B, min.B, max.B));
		}

		public static Vector4 Clamp(Vector4 rgba, Vector4 min, Vector4 max)
		{
			return new Vector4(MathUtils.Clamp(rgba.X, min.X, max.X),
			                   MathUtils.Clamp(rgba.Y, min.Y, max.Y),
			                   MathUtils.Clamp(rgba.Z, min.Z, max.Z),
			                   MathUtils.Clamp(rgba.W, min.W, max.W));
		}

		#endregion
	}
}
