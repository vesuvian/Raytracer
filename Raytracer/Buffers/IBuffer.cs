using System;
using System.Drawing;
using Raytracer.Utils;

namespace Raytracer.Buffers
{
	public interface IBuffer : IDisposable
	{
		int Height { get; }
		int Width { get; }
		void SetPixel(int x, int y, Color color);
		Color GetPixel(int x, int y);
	}

	public static class BufferExtensions
	{
		public static Color GetPixelBilinear(this IBuffer extends, float x, float y)
		{
			int xMin = (int)x;
			int yMin = (int)y;
			int xMax = xMin == extends.Width - 1 ? 0 : xMin + 1;
			int yMax = yMin == extends.Height - 1 ? 0 : yMin + 1;

			Color c00 = extends.GetPixel(xMin, yMin);
			Color c01 = extends.GetPixel(xMin, yMax);
			Color c10 = extends.GetPixel(xMax, yMin);
			Color c11 = extends.GetPixel(xMax, yMax);

			float deltaX = x - xMin;
			float deltaY = y - yMin;

			Color a = ColorUtils.Add(ColorUtils.Multiply(c00, 1 - deltaX), ColorUtils.Multiply(c10, deltaX));
			Color b = ColorUtils.Add(ColorUtils.Multiply(c01, 1 - deltaX), ColorUtils.Multiply(c11, deltaX));
			return ColorUtils.Add(ColorUtils.Multiply(a, 1 - deltaY), ColorUtils.Multiply(b, deltaY));
		}
	}
}