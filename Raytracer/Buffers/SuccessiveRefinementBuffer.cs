using System;
using System.Drawing;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.Buffers
{
	public sealed class SuccessiveRefinementBuffer : AbstractBuffer
	{
		private readonly IBuffer m_Buffer;
		private readonly Vector3[] m_Pixels;
		private readonly int[] m_PixelSamples;
        private readonly Aabb[] m_Variance;

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
            m_Variance = new Aabb[m_Buffer.Width * m_Buffer.Height];

            for (int index = 0; index < buffer.Width * buffer.Height; index++)
            {
                m_Variance[index] = new Aabb
                {
                    Min = new Vector3(float.MaxValue),
                    Max = new Vector3(float.MinValue)
                };
            }
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
			var index = x + y * Width;
            var rgb = color.ToRgb();

			lock (m_Pixels)
			{
				// Update the pixel value
				m_Pixels[index] += rgb;
				m_PixelSamples[index]++;

				// Update variance
                var bounds = m_Variance[index];
                bounds.Min = Vector3.Min(bounds.Min, rgb);
                bounds.Max = Vector3.Max(bounds.Max, rgb);
                m_Variance[index] = bounds;

				// Write to the underlying buffer
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

        public bool HasVariance(int x, int y)
        {
            const float maxMagnitude = 2.0f / 256.0f;

            int index = x + y * Width;

            lock (m_Pixels)
            {
                var bounds = m_Variance[index];
				var magnitude = (bounds.Max - bounds.Min).Length();
                return magnitude >= maxMagnitude;
            }
		}
	}
}
