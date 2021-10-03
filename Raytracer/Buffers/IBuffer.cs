using System;
using System.Drawing;
using System.Numerics;
using Raytracer.Extensions;

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

			Vector3 c00 = extends.GetPixel(xMin, yMin).ToRgb();
			Vector3 c01 = extends.GetPixel(xMin, yMax).ToRgb();
			Vector3 c10 = extends.GetPixel(xMax, yMin).ToRgb();
			Vector3 c11 = extends.GetPixel(xMax, yMax).ToRgb();

			float deltaX = x - xMin;
			float deltaY = y - yMin;

			Vector3 a = c00 * (1 - deltaX) + c10 * deltaX;
			Vector3 b = c01 * (1 - deltaX) + c11 * deltaX;
			Vector3 rgb = a * (1 - deltaY) + b * deltaY;

			return rgb.FromRgbToColor();
		}
	}
}