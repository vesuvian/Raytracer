using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Raytracer.Buffers;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.Layers
{
	public abstract class AbstractLayer : ILayer
	{
		public float Gamma { get; set; } = 1.0f;

		public event EventHandler OnProgressChanged;

		private ulong m_Progress;

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

		public virtual void Render(Scene scene, IBuffer buffer, CancellationToken cancellationToken = default)
		{
			IBuffer filter = new MedianFilterBuffer(buffer);
			IBuffer successiveBuffer = new SuccessiveRefinementBuffer(filter);

			int width = successiveBuffer.Width;
			int height = successiveBuffer.Height;

			Rectangle region = new Rectangle(0, 0, width, height);

			Start = DateTime.UtcNow;
			Progress = 0;
			RenderSize = (ulong)region.Width * (ulong)region.Height * (ulong)scene.Camera.Samples;

			ulong pixelsComplete = 0;

			IEnumerable<Tuple<int, int, int>> pixels =
				Enumerable.Range(0, scene.Camera.Samples)
				          .SelectMany(s => Enumerable.Range(0, region.Width * region.Height)
				                                     .Select(px =>
				                                     {
					                                     int feistel = FeistelNet(px, region.Width, region.Height);
					                                     int x = region.Left + feistel % region.Width;
					                                     int y = region.Top + feistel / region.Width;

														 return new Tuple<int, int, int>(s, x, y);
				                                     }));

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
						cancellationToken.ThrowIfCancellationRequested();

						int iteration = px.Item1;
						int x = px.Item2;
						int y = px.Item3;
                        Random random = new Random(HashCode.Combine(iteration, x, y));
                        
                        Vector3 sample = Sample(scene, buffer, x, y, random, cancellationToken);

						// Gamma Correction
						sample = new Vector3(MathF.Pow(MathF.Max(0, sample.X), 1 / Gamma),
						                     MathF.Pow(MathF.Max(0, sample.Y), 1 / Gamma),
						                     MathF.Pow(MathF.Max(0, sample.Z), 1 / Gamma));

                        successiveBuffer.SetPixel(x, y, sample.FromRgbToColor());
						Progress = pixelsComplete++;
					}
					catch (OperationCanceledException)
					{
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

        protected virtual Vector3 Sample(Scene scene, IBuffer buffer, int x, int y, Random random,
                                         CancellationToken cancellationToken = default)
        {
            float xViewportMin = x / (float)buffer.Width;
            float xViewportMax = (x + 1) / (float)buffer.Width;
            float yViewportMin = y / (float)buffer.Height;
            float yViewportMax = (y + 1) / (float)buffer.Height;
			
            Ray ray = scene.Camera.CreateRay(xViewportMin, xViewportMax, yViewportMin, yViewportMax, random);

			CastRay(scene, ray, random, 0, Vector3.One, out Vector3 sample, cancellationToken);
            return sample;
        }

        protected abstract bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
                                        out Vector3 sample, CancellationToken cancellationToken = default);

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
