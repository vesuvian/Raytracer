using System.Drawing;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Buffers
{
	public sealed class SuccessiveRefinementBuffer : AbstractBuffer
	{
		private readonly IBuffer m_Buffer;
		private readonly Vector3[] m_Pixels;
		private readonly int[] m_PixelSamples;

		public override int Height { get { return m_Buffer.Height; } }
		public override int Width { get { return m_Buffer.Width; } }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="buffer"></param>
		public SuccessiveRefinementBuffer(IBuffer buffer)
		{
			m_Buffer = buffer;
			m_Pixels = new Vector3[m_Buffer.Width * m_Buffer.Height];
			m_PixelSamples = new int[m_Buffer.Width * m_Buffer.Height];
		}

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
		{
			m_Buffer.Dispose();
		}

		public override void SetPixel(int x, int y, Color color)
		{
			int index = x + y * Width;

			lock (m_Pixels)
			{
				m_Pixels[index] += color.ToRgb();
				m_PixelSamples[index]++;
				m_Buffer.SetPixel(x, y, GetPixel(x, y));
			}
		}

		public override Color GetPixel(int x, int y)
		{
			int index = x + y * Width;

			lock (m_Pixels)
			{
				Vector3 sum = m_Pixels[index];
				int count = m_PixelSamples[index];
				Vector3 average = count == 0 ? default : sum / count;

				return average.FromRgbToColor();
			}
		}
	}
}
