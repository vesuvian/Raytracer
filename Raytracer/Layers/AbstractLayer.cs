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
        public event EventHandler OnProgressChanged;

        private ulong m_Progress;

        public float Gamma { get; set; } = 1.0f;

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
            //buffer = new MedianFilterBuffer(buffer);
            var successiveBuffer = new SuccessiveRefinementBuffer(buffer);

            int width = successiveBuffer.Width;
            int height = successiveBuffer.Height;

            Rectangle region = new Rectangle(0, 0, width, height);

            Start = DateTime.UtcNow;
            Progress = 0;
            RenderSize = (ulong)region.Width * (ulong)region.Height * (ulong)scene.Samples;

            ulong pixelsComplete = 0;

            IEnumerable<Tuple<int, int, int>> pixels = EnumeratePixels(region, scene.Samples);

            ParallelOptions po = new ParallelOptions
            {
                CancellationToken = cancellationToken
            };

            try
            {
                Parallel.ForEach(pixels, po, px =>
                {
                    var (s, x, y) = px;

                    var priority = Thread.CurrentThread.Priority;
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                    try
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        // Skip pixels with low variance
                        var variance = s < 4 || successiveBuffer.HasVariance(x, y);

                        if (variance)
                        {
                            // Makes each sample deterministic
                            Random random = new Random(HashCode.Combine(s, x, y));
                            Vector3 sample = Sample(scene, buffer, x, y, random, cancellationToken);
                            sample = GammaCorrection(sample);
                            successiveBuffer.SetPixel(x, y, sample.FromRgbToColor());
                        }

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

                Progress = RenderSize;
            }
            catch (OperationCanceledException)
            {
            }

            End = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns an RGB value for the given x,y co-ordinate in buffer space.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="buffer"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="random"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual Vector3 Sample(Scene scene, IBuffer buffer, int x, int y, Random random,
                                         CancellationToken cancellationToken = default)
        {
            float xViewportMin = x / (float)buffer.Width;
            float xViewportMax = (x + 1) / (float)buffer.Width;
            float yViewportMin = y / (float)buffer.Height;
            float yViewportMax = (y + 1) / (float)buffer.Height;

            // Don't randomize the rays if we're doing a single pass
            if (scene.Samples == 1)
            {
                xViewportMin = (xViewportMin + xViewportMax) / 2;
                xViewportMax = xViewportMin;
                yViewportMin = (yViewportMin + yViewportMax) / 2;
                yViewportMax = yViewportMin;
            }

            Ray ray = scene.Camera.CreateRay(xViewportMin, xViewportMax, yViewportMin, yViewportMax, random);

            CastRay(scene, ray, random, 0, Vector3.One, out Vector3 sample, cancellationToken);
            return sample;
        }

        /// <summary>
        /// Casts a ray into the scene and returns true if there was a collision.
        /// Outputs an RGB value for the ray.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="ray"></param>
        /// <param name="random"></param>
        /// <param name="rayDepth"></param>
        /// <param name="rayWeight"></param>
        /// <param name="sample"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract bool CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
                                        out Vector3 sample, CancellationToken cancellationToken = default);

        /// <summary>
        /// Loops over each pixel in the render region, visiting each pixel the given number of samples.
        /// </summary>
        /// <param name="region"></param>
        /// <param name="samples"></param>
        /// <returns></returns>
        private static IEnumerable<Tuple<int, int, int>> EnumeratePixels(Rectangle region, int samples)
        {
            return
                Enumerable.Range(0, samples)
                          .SelectMany(s => Enumerable.Range(0, region.Width * region.Height)
                                                     .Select(px =>
                                                     {
                                                         int feistel = FeistelNet(px, region.Width, region.Height);
                                                         int x = region.Left + feistel % region.Width;
                                                         int y = region.Top + feistel / region.Width;

                                                         return new Tuple<int, int, int>(s, x, y);
                                                     }));
        }

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

        /// <summary>
        /// Applies the configured Gamma correction to the RGB value.
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        private Vector3 GammaCorrection(Vector3 rgb)
        {
            return new Vector3(MathF.Pow(MathF.Max(0, rgb.X), 1 / Gamma),
                               MathF.Pow(MathF.Max(0, rgb.Y), 1 / Gamma),
                               MathF.Pow(MathF.Max(0, rgb.Z), 1 / Gamma));
        }
    }
}
