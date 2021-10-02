﻿using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Buffers
{
	public sealed class SuccessiveRefinementBuffer : AbstractBuffer
	{
		private readonly IBuffer m_Buffer;
		private readonly Vector4[] m_Pixels;
		private readonly int[] m_PixelSamples;

		public override int Height { get { return m_Buffer.Height; } }
		public override int Width { get { return m_Buffer.Width; } }

		public SuccessiveRefinementBuffer(IBuffer buffer)
		{
			m_Buffer = buffer;
			m_Pixels = new Vector4[m_Buffer.Width * m_Buffer.Height];
			m_PixelSamples = new int[m_Buffer.Width * m_Buffer.Height];
		}

		public override void Dispose()
		{
			m_Buffer.Dispose();
		}

		public override void SetPixel(int x, int y, Color color)
		{
			int index = x + y * Width;

			lock (m_Pixels)
			{
				m_Pixels[index] += ColorUtils.ColorToRgb(color);
				m_PixelSamples[index]++;
				m_Buffer.SetPixel(x, y, GetPixel(x, y));
			}
		}

		public override Color GetPixel(int x, int y)
		{
			int index = x + y * Width;

			lock (m_Pixels)
			{
				Vector4 sum = m_Pixels[index];
				int count = m_PixelSamples[index];
				Vector4 average = count == 0 ? default : sum / count;

				return ColorUtils.RgbToColor(average);
			}
		}
	}
}
