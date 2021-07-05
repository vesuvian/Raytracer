using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public abstract class AbstractLayer : ILayer
	{
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

			Start = DateTime.UtcNow;
			Progress = 0;
			RenderSize = width * height;

			int pixelsComplete = 0;

			IEnumerable<int> pixels =
				Enumerable.Range(0, width * height)
				          .Select(px => FeistelNet(px, width, height));

			Parallel.ForEach(pixels, px =>
			{
				int x = px % width;
				int y = px / width;

				float xViewportMin = x / (float)width;
				float xViewportMax = (x + 1) / (float)width;
				float yViewportMin = y / (float)height;
				float yViewportMax = (y + 1) / (float)height;

				IEnumerable<Vector4> samples =
					scene.Camera
					     .CreateRays(xViewportMin, xViewportMax, yViewportMin, yViewportMax)
					     .Select(r => CastRay(scene, r, 0));

				Vector4 pixel = ColorUtils.Average(samples);
				buffer.SetPixel(x, y, ColorUtils.ToColorRgba(pixel));
				Progress = pixelsComplete++;
			});

			End = DateTime.UtcNow;
		}

		protected abstract Vector4 CastRay(Scene scene, Ray ray, int rayDepth);

		/// <summary>
		/// Gets a "random" pixel for each input, only visiting each pixel once.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		private static int FeistelNet(int index, int width, int height)
		{
			for (int i = 0; i < 8; i++)
			{
				int l = (index / width) | 0;
				int r = index % width;
				int nl = r;
				int f = (r * 356357 + i * 1234567) % height;
				r = (l + f) % height;
				l = nl;
				index = height * l + r;
			}
			return index;
		}
	}
}
