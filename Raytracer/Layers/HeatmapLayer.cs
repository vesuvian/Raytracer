using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Threading;
using Raytracer.Buffers;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class HeatmapLayer : AbstractMaterialsLayer
    {
        private float m_AverageTicks;

        private readonly Gradient m_Gradient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HeatmapLayer()
        {
            m_Gradient = new Gradient();

            m_Gradient.Add(0, Color.Black.ToRgba());
            m_Gradient.Add(0.33f, Color.DodgerBlue.ToRgba());
            m_Gradient.Add(0.66f, Color.Yellow.ToRgba());
            m_Gradient.Add(1, Color.Red.ToRgba());
        }

        protected override Vector3 Sample(Scene scene, IBuffer buffer, int x, int y, Random random,
                                          CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();

            base.Sample(scene, buffer, x, y, random, cancellationToken);

            var ticks = stopwatch.ElapsedTicks;
            m_AverageTicks = ApproxRollingAverage(m_AverageTicks, ticks);
            var delta = MathUtils.Clamp(ticks / (m_AverageTicks * 2), 0, 1);

            return m_Gradient.Sample(delta).ToVector3();
        }

        private static float ApproxRollingAverage(float average, float newSample)
        {
            const int samples = 1000;

            average -= average / samples;
            average += newSample / samples;

            return average;
        }
    }
}
