using System;
using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Extensions
{
	public static class ColorExtensions
	{
		private const float EPSILON = 0.00001f;

		/// <summary>
		/// Gets the CIE D65 (white) structure.
		/// </summary>
		public static Vector3 XyzD65White = new Vector3(0.9505f, 1.0f, 1.0890f);

		#region RGB

		/// <summary>
		/// Converts Color to RGBA.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Vector4 ToRgba(this Color extends)
		{
			return new Vector4(extends.R, extends.G, extends.B, extends.A) / 255;
		}

		/// <summary>
		/// Converts Color to RGB.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Vector3 ToRgb(this Color extends)
		{
			return new Vector3(extends.R, extends.G, extends.B) / 255;
		}

		/// <summary>
		/// Converts RGBA to Color.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Color FromRgbaToColor(this Vector4 extends)
		{
			return Color.FromArgb((int)MathUtils.Clamp(extends.W * 255, 0, 255),
			                      (int)MathUtils.Clamp(extends.X * 255, 0, 255),
			                      (int)MathUtils.Clamp(extends.Y * 255, 0, 255),
			                      (int)MathUtils.Clamp(extends.Z * 255, 0, 255));
		}

		/// <summary>
		/// Converts RGB to Color.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Color FromRgbToColor(this Vector3 extends)
		{
			return Color.FromArgb((int)MathUtils.Clamp(extends.X * 255, 0, 255),
			                      (int)MathUtils.Clamp(extends.Y * 255, 0, 255),
			                      (int)MathUtils.Clamp(extends.Z * 255, 0, 255));
		}

		#endregion

		#region HSL

		/// <summary>
		/// Converts RGBA to HSLA.
		/// </summary>
		public static Vector4 FromRgbaToHsla(this Vector4 extends)
		{
			float r = extends.X;
			float g = extends.Y;
			float b = extends.Z;
			float a = extends.W;

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
		/// Converts RGB to HSL.
		/// </summary>
		public static Vector3 FromRgbToHsl(this Vector3 extends)
		{
			return extends.ToVector4()
			              .FromRgbaToHsla()
			              .ToVector3();
		}

		/// <summary>
		/// Converts from HSLA to RGBA.
		/// </summary>
		/// <param name="extends"></param>
		public static Vector4 FromHslaToRgba(this Vector4 extends)
		{
			float h = MathUtils.ModPositive(extends.X, 360);
			float s = MathUtils.Clamp(extends.Y, 0, 1);
			float l = MathUtils.Clamp(extends.Z, 0, 1);
			float a = MathUtils.Clamp(extends.W, 0, 1);

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

		/// <summary>
		/// Converts HSL to RGB.
		/// </summary>
		public static Vector3 FromHslToRgb(this Vector3 extends)
		{
			return extends.ToVector4()
			              .FromHslaToRgba()
			              .ToVector3();
		}

		#endregion

		#region LAB

		/// <summary>
		/// Converts from RGBA to LABA.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="rgbWhite"></param>
		/// <returns></returns>
		public static Vector4 FromRgbaToLaba(this Vector4 extends, Vector3 rgbWhite)
		{
			Vector4 xyz = extends.FromRgbaToXyza();
			Vector3 xyzWhite = rgbWhite.ToVector4(1).FromRgbaToXyza().ToVector3();
			return xyz.FromXyzaToLaba(xyzWhite);
		}

		/// <summary>
		/// Converts from RGB to LAB.
		/// </summary>
		/// <param name="extends"></param>
		/// <param name="rgbWhite"></param>
		/// <returns></returns>
		public static Vector3 FromRgbToLab(this Vector3 extends, Vector3 rgbWhite)
		{
			Vector3 xyz = extends.FromRgbToXyz();
			Vector3 xyzWhite = rgbWhite.FromRgbToXyz();
			return xyz.FromXyzToLab(xyzWhite);
		}

		/// <summary>
		/// Converts from RGBA to LABA.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Vector4 FromRgbaToLaba(this Vector4 extends)
		{
			Vector4 xyz = extends.FromRgbaToXyza();
			return xyz.FromXyzaToLaba(XyzD65White);
		}

		/// <summary>
		/// Converts from RGB to LAB.
		/// </summary>
		/// <param name="extends"></param>
		/// <returns></returns>
		public static Vector3 FromRgbToLab(this Vector3 extends)
		{
			Vector3 xyz = extends.FromRgbToXyz();
			return xyz.FromXyzToLab(XyzD65White);
		}

		#endregion

		#region XYZ

		/// <summary>
		/// Converts RGBA to XYZA.
		/// </summary>
		public static Vector4 FromRgbaToXyza(this Vector4 extends)
		{
			// convert to a sRGB form
			double r =
				extends.X > 0.04045
					? System.Math.Pow((extends.X + 0.055) / (1 + 0.055), 2.2)
					: extends.X / 12.92;

			double g =
				extends.Y > 0.04045
					? System.Math.Pow((extends.Y + 0.055) / (1 + 0.055), 2.2)
					: extends.Y / 12.92;

			double b =
				extends.Z > 0.04045
					? System.Math.Pow((extends.Z + 0.055) / (1 + 0.055), 2.2)
					: extends.Z / 12.92;

			// converts
			return new Vector4((float)(r * 0.4124 + g * 0.3576 + b * 0.1805),
							  (float)(r * 0.2126 + g * 0.7152 + b * 0.0722),
							  (float)(r * 0.0193 + g * 0.1192 + b * 0.9505),
							  extends.W);
		}

		/// <summary>
		/// Converts RGB to XYZ.
		/// </summary>
		public static Vector3 FromRgbToXyz(this Vector3 extends)
		{
			return extends.ToVector4(1)
			              .FromRgbaToXyza()
			              .ToVector3();
		}

		/// <summary>
		/// Converts XYZA to LABA.
		/// </summary>
		public static Vector4 FromXyzaToLaba(this Vector4 extends, Vector3 xyzWhite)
		{
			// XYZ to L*a*b* transformation function.
			Func<double, double> fxyz = t =>
				((t > 0.008856)
					? System.Math.Pow(t, (1.0 / 3.0))
					: (7.787 * t + 16.0 / 116.0));

			return new Vector4((float)(116.0 * fxyz(extends.Y / xyzWhite.Y) - 16),
							   (float)(500.0 * (fxyz(extends.X / xyzWhite.X) - fxyz(extends.Y / xyzWhite.Y))),
							   (float)(200.0 * (fxyz(extends.Y / xyzWhite.Y) - fxyz(extends.Z / xyzWhite.Z))),
							   extends.W);
		}

		/// <summary>
		/// Converts XYZ to LAB.
		/// </summary>
		public static Vector3 FromXyzToLab(this Vector3 extends, Vector3 xyzWhite)
		{
			return extends.ToVector4(1)
			              .FromXyzaToLaba(xyzWhite)
			              .ToVector3();
		}

		#endregion
	}
}
