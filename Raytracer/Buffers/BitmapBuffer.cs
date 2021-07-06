using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Raytracer.Buffers
{
	public sealed class BitmapBuffer : IBuffer
	{
		private GCHandle m_BitsHandle;

		public Bitmap Bitmap { get; private set; }
		public int[] Bits { get; private set; }
		public bool Disposed { get; private set; }
		public int Height { get; private set; }
		public int Width { get; private set; }

		public BitmapBuffer(int width, int height)
		{
			Width = width;
			Height = height;
			Bits = new int[width * height];
			m_BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
			Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, m_BitsHandle.AddrOfPinnedObject());
		}

		public static BitmapBuffer FromBitmap(Bitmap bitmap)
		{
			BitmapBuffer output = new BitmapBuffer(bitmap.Width, bitmap.Height);

			using (Graphics graphics = Graphics.FromImage(output.Bitmap))
				graphics.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);

			return output;
		}

		public void Dispose()
		{
			if (Disposed)
				return;

			Disposed = true;
			Bitmap.Dispose();
			m_BitsHandle.Free();
		}

		public void SetPixel(int x, int y, Color color)
		{
			int index = x + (y * Width);
			int col = color.ToArgb();

			Bits[index] = col;
		}

		public Color GetPixel(int x, int y)
		{
			int index = x + (y * Width);
			int col = Bits[index];
			Color result = Color.FromArgb(col);

			return result;
		}
	}
}