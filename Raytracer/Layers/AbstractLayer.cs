using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Raytracer.Math;

namespace Raytracer.Layers
{
	public abstract class AbstractLayer : ILayer
	{
		// 128 threads
		private const int BUCKETS_X = 16;
		private const int BUCKETS_Y = 16;

		public void Render(Scene scene, Bitmap buffer)
		{
			int width = buffer.Width;
			int height = buffer.Height;

			IEnumerable<Rectangle> buckets = GetBuckets(width, height, BUCKETS_X, BUCKETS_Y);

			Parallel.ForEach(buckets, bucket =>
			{
				for (int y = bucket.Top; y < bucket.Bottom; y++)
				{
					for (int x = bucket.Left; x < bucket.Right; x++)
					{
						float xViewport = (x + 0.5f) / width;
						float yViewport = (y + 0.5f) / height;

						Ray ray = scene.Camera.CreateRay(xViewport, yViewport);
						Color pixel = CastRay(scene, ray);
						lock (buffer)
							buffer.SetPixel(x, y, pixel);
					}
				}
			});
		}

		private static IEnumerable<Rectangle> GetBuckets(int width, int height, int bucketsX, int bucketsY)
		{
			int bucketWidth = width / bucketsX;
			int bucketHeight = height / bucketsY;

			for (int x = 0; x < bucketsX; x++)
			{
				for (int y = 0; y < bucketsY; y++)
				{
					// TODO - Better handle rects that don't divide evenly?
					yield return new Rectangle(x * bucketWidth, y * bucketHeight, bucketWidth, bucketHeight);
				}
			}
		}

		protected abstract Color CastRay(Scene scene, Ray ray);
	}
}
