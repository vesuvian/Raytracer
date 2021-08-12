using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Raytracer.Buffers;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public abstract class AbstractLayer : ILayer
	{
		public event EventHandler OnProgressChanged;

		private ulong m_Progress;

		protected Random Random { get; } = new Random();

		public DateTime Start { get; private set; }
		public DateTime End { get; private set; }

		public ulong Progress
		{
			get { return m_Progress; }
			private set
			{
				m_Progress = value;

				OnProgressChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public ulong RenderSize { get; private set; }

		public void Render(Scene scene, IBuffer buffer, CancellationToken cancellationToken = default)
		{
			IBuffer successiveBuffer = new SuccessiveRefinementBuffer(buffer);

			int width = successiveBuffer.Width;
			int height = successiveBuffer.Height;

			Rectangle region = new Rectangle(0, 0, width, height);

			Start = DateTime.UtcNow;
			Progress = 0;
			RenderSize = (ulong)region.Width * (ulong)region.Height * (ulong)scene.Camera.Samples;

			ulong pixelsComplete = 0;

			IEnumerable<int> pixels =
				Enumerable.Range(0, scene.Camera.Samples)
				          .SelectMany(_ => Enumerable.Range(0, region.Width * region.Height)
				                                     .Select(px => FeistelNet(px, region.Width, region.Height)));

			ParallelOptions po = new ParallelOptions
			{
				CancellationToken = cancellationToken
			};

			try
			{
				Parallel.ForEach(pixels, po, px =>
				{
					var priority = Thread.CurrentThread.Priority;
					Thread.CurrentThread.Priority = ThreadPriority.Lowest;

					try
					{
						int x = region.Left + px % region.Width;
						int y = region.Top + px / region.Width;

						float xViewportMin = x / (float)width;
						float xViewportMax = (x + 1) / (float)width;
						float yViewportMin = y / (float)height;
						float yViewportMax = (y + 1) / (float)height;

						Ray ray;
						lock (Random)
							ray = scene.Camera.CreateRay(xViewportMin, xViewportMax, yViewportMin, yViewportMax, Random);
						Vector4 sample = CastRay(scene, ray, 0);

						successiveBuffer.SetPixel(x, y, ColorUtils.ToColorRgba(sample));
						Progress = pixelsComplete++;
					}
					finally
					{
						Thread.CurrentThread.Priority = priority;
					}
				});
			}
			catch (OperationCanceledException)
			{
			}

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
