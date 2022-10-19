using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Threading;
using Raytracer.Buffers;
using Raytracer.Extensions;
using Raytracer.Geometry;
using Raytracer.Utils;

namespace Raytracer.Layers
{
	public sealed class VarianceLayer : AbstractMaterialsLayer
    {
        private readonly Gradient m_Gradient;
        private readonly List<Aabb> m_Variance = new();

        private float m_MaxMagnitude;

        /// <summary>
        /// Constructor.
        /// </summary>
        public VarianceLayer()
        {
            m_Gradient = new Gradient();

            m_Gradient.Add(0, Color.Black.ToRgba());
            m_Gradient.Add(0.33f, Color.DodgerBlue.ToRgba());
            m_Gradient.Add(0.66f, Color.Yellow.ToRgba());
            m_Gradient.Add(1, Color.Red.ToRgba());
        }

        public override void Render(Scene scene, IBuffer buffer, CancellationToken cancellationToken = default)
        {
            // Setup the variance buffer
            lock (m_Variance)
            {
                m_MaxMagnitude = 0;
                m_Variance.Clear();

                for (int index = 0; index < buffer.Width * buffer.Height; index++)
                {
                    m_Variance.Add(new Aabb
                    {
                        Min = new Vector3(float.MaxValue),
                        Max = new Vector3(float.MinValue)
                    });
                }
            }

            base.Render(scene, buffer, cancellationToken);
        }

        protected override Vector3 Sample(Scene scene, IBuffer buffer, int x, int y, Random random,
                                          CancellationToken cancellationToken = default)
        {
            var sample = base.Sample(scene, buffer, x, y, random, cancellationToken);
            sample = Vector3.Min(sample, Vector3.One);

            var delta = 0.0f;

            lock (m_Variance)
            {
                var index = x + y * buffer.Width;

                var bounds = m_Variance[x + y * buffer.Width];
                bounds.Min = Vector3.Min(bounds.Min, sample);
                bounds.Max = Vector3.Max(bounds.Max, sample);
                m_Variance[index] = bounds;

                var magnitude = (bounds.Max - bounds.Min).Length();
                m_MaxMagnitude = MathF.Max(magnitude, m_MaxMagnitude);

                delta = m_MaxMagnitude == 0 ? 0 : magnitude / m_MaxMagnitude;
            }

            return m_Gradient.Sample(delta).ToVector3();
        }
    }
}
