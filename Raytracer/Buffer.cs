using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Raytracer.Utils;

namespace Raytracer
{
	public sealed class Buffer : IDisposable
	{
		private GCHandle m_BitsHandle;

		public Bitmap Bitmap { get; private set; }
		public int[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		public Buffer(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new int[width * height];
			m_BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, m_BitsHandle.AddrOfPinnedObject());
		}

		public static Buffer FromBitmap(Bitmap bitmap)
		{
			Buffer output = new Buffer(bitmap.Width, bitmap.Height);

			using (Graphics graphics = Graphics.FromImage(output.Bitmap))
				graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);

			return output;
		}

		public void Dispose()
		{
			if (Disposed) return;
			Disposed = true;
			Bitmap.Dispose();
			m_BitsHandle.Free();
		}

		public void SetPixel(int x, int y, Color colour)
		{
			int index = x + (y * Width);
			int col = colour.ToArgb();

			Bits[index] = col;
		}

		public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}

		public Color GetPixelBilinear(float x, float y)
		{
			int xMin = (int)x;
			int yMin = (int)y;
			int xMax = xMin == Width - 1 ? 0 : xMin + 1;
			int yMax = yMin == Height - 1 ? 0 : yMin + 1;

			Color c00 = GetPixel(xMin, yMin);
			Color c01 = GetPixel(xMin, yMax);
			Color c10 = GetPixel(xMax, yMin);
			Color c11 = GetPixel(xMax, yMax);

			float deltaX = x - xMin;
			float deltaY = y - yMin;

			Color a = ColorUtils.Add(ColorUtils.Multiply(c00, 1 - deltaX), ColorUtils.Multiply(c10, deltaX));
			Color b = ColorUtils.Add(ColorUtils.Multiply(c01, 1 - deltaX), ColorUtils.Multiply(c11, deltaX));
			return ColorUtils.Add(ColorUtils.Multiply(a, 1 - deltaY), ColorUtils.Multiply(b, deltaY));
		}
	}
}