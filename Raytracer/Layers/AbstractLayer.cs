using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public abstract class AbstractLayer : ILayer
	{
		// 128 threads
		private const int BUCKETS_X = 16;
		private const int BUCKETS_Y = 16;

		public event EventHandler OnProgressChanged;

		private int m_Progress;

		public DateTime Start { get; private set; }
		public DateTime End { get; private set; }

		public int Progress
		{
			get { return m_Progress; }
			private set
			{
				m_Progress = value;

				OnProgressChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public int RenderSize { get; private set; }

		public void Render(Scene scene, Buffer buffer)
		{
			int width = buffer.Width;
			int height = buffer.Height;

			IEnumerable<Rectangle> buckets = GetBuckets(0, 0, width, height, BUCKETS_X, BUCKETS_Y);

			Start = DateTime.UtcNow;
			Progress = 0;
			RenderSize = width * height;

			int pixelsComplete = 0;

			Parallel.ForEach(buckets, bucket =>
			{
				for (int y = bucket.Top; y < bucket.Bottom; y++)
				{
					for (int x = bucket.Left; x < bucket.Right; x++)
					{
						float xViewportMin = x / (float)width;
						float xViewportMax = (x + 1) / (float)width;
						float yViewportMin = y / (float)height;
						float yViewportMax = (y + 1) / (float)height;

						IEnumerable<Color> samples =
							scene.Camera
							     .CreateRays(xViewportMin, xViewportMax, yViewportMin, yViewportMax)
							     .Select(r => CastRay(scene, r, 0));

						Color pixel = ColorUtils.Average(samples);
						buffer.SetPixel(x, y, pixel);
						Progress = pixelsComplete++;
					}
				}
			});

			End = DateTime.UtcNow;
		}

		private static IEnumerable<Rectangle> GetBuckets(int x, int y, int width, int height, int bucketsX, int bucketsY)
		{
			bucketsX = System.Math.Min(bucketsX, width);
			bucketsY = System.Math.Min(bucketsY, height);

			int bucketWidth = width / bucketsX;
			int bucketHeight = height / bucketsY;

			for (int bucketX = 0; bucketX < bucketsX; bucketX++)
			{
				for (int bucketY = 0; bucketY < bucketsY; bucketY++)
				{
					int left = x + bucketX * bucketWidth;
					int top = y + bucketY * bucketHeight;
					int rectWidth = bucketWidth;
					int rectHeight = bucketHeight;

					if (bucketX == bucketsX - 1)
						rectWidth = width - (left - x);

					if (bucketY == bucketsY - 1)
						rectHeight = height - (top - y);
					
					yield return new Rectangle(left, top, rectWidth, rectHeight);
				}
			}
		}

		protected abstract Color CastRay(Scene scene, Ray ray, int rayDepth);
	}
}
