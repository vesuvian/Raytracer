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

			Parallel.ForEach(Enumerable.Range(0, width * height), px =>
			{
				int x = px % width;
				int y = px / width;

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
			});

			End = DateTime.UtcNow;
		}

		protected abstract Color CastRay(Scene scene, Ray ray, int rayDepth);
	}
}
