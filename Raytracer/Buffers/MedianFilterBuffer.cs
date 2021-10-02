using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Raytracer.Buffers
{
    public sealed class MedianFilterBuffer : AbstractBuffer
    {
		private readonly IBuffer m_Buffer;
		private readonly Color?[] m_Cache;

		public override int Height { get { return m_Buffer.Height; } }
		public override int Width { get { return m_Buffer.Width; } }

		public MedianFilterBuffer(IBuffer buffer)
		{
			m_Buffer = buffer;
			m_Cache = new Color?[Width * Height];
		}

		public override void Dispose()
		{
			m_Buffer.Dispose();
		}

		public override void SetPixel(int x, int y, Color color)
		{
			int index = y * Width + x;

			lock (m_Cache)
			{
				// No change
				if (color == m_Cache[index])
					return;

				m_Cache[index] = color;

				UpdatePixel(x - 1, y - 1, color);
				UpdatePixel(x, y - 1, color);
				UpdatePixel(x + 1, y - 1, color);
				UpdatePixel(x - 1, y, color);
				UpdatePixel(x, y, color);
				UpdatePixel(x + 1, y, color);
				UpdatePixel(x - 1, y + 1, color);
				UpdatePixel(x, y + 1, color);
				UpdatePixel(x + 1, y + 1, color);
			}
		}

        private void UpdatePixel(int x, int y, Color color)
        {
			if (x < 0 || x >= Width || y < 0 || y >= Height)
				return;

			lock (m_Buffer)
			{
				// Get the neighboring pixels as an array
				Color[] pixels = new Color[9];

				pixels[0] = GetCacheOrDefault(x - 1, y - 1, color);
				pixels[1] = GetCacheOrDefault(x, y - 1, color);
				pixels[2] = GetCacheOrDefault(x + 1, y - 1, color);
				pixels[3] = GetCacheOrDefault(x - 1, y, color);
				pixels[4] = GetCacheOrDefault(x, y, color);
				pixels[5] = GetCacheOrDefault(x + 1, y, color);
				pixels[6] = GetCacheOrDefault(x - 1, y + 1, color);
				pixels[7] = GetCacheOrDefault(x, y + 1, color);
				pixels[8] = GetCacheOrDefault(x + 1, y + 1, color);

				// Sort
				Array.Sort(pixels, ColorChannelComparer.Red);
				int red = pixels[4].R;
				Array.Sort(pixels, ColorChannelComparer.Green);
				int green = pixels[4].G;
				Array.Sort(pixels, ColorChannelComparer.Blue);
				int blue = pixels[4].B;

				// Use the median value
				m_Buffer.SetPixel(x, y, Color.FromArgb(red, green, blue));
			}
		}

        private Color GetCacheOrDefault(int x, int y, Color def)
        {
			int index = y * Width + x;
			return x < 0 || x >= Width || y < 0 || y >= Height ? def : m_Cache[index] ?? def;
        }

        public override Color GetPixel(int x, int y)
		{
			lock (m_Buffer)
				return m_Buffer.GetPixel(x, y);
		}

		private sealed class ColorChannelComparer : IComparer<Color>
        {
			public static ColorChannelComparer Red { get; } = new ColorChannelComparer(c => c.R);
			public static ColorChannelComparer Green { get; } = new ColorChannelComparer(c => c.G);
			public static ColorChannelComparer Blue { get; } = new ColorChannelComparer(c => c.B);

			private readonly Func<Color, int> m_GetChannel;

			public ColorChannelComparer(Func<Color, int> getChannel)
            {
				m_GetChannel = getChannel;
            }

			public int Compare([AllowNull] Color x, [AllowNull] Color y)
            {
                if (x == null && y == null)
					return 0;

				if (x == null)
					return -1;

				if (y == null)
					return 1;

				int channelX = m_GetChannel(x);
				int channelY = m_GetChannel(y);

				if (channelX == channelY)
					return 0;

				if (channelX > channelY)
					return 1;

				return -1;
			}
        }
	}
}
