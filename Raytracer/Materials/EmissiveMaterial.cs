using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public sealed class EmissiveMaterial : AbstractMaterial
	{
		public ITexture Emission { get; set; } = new SolidColorTexture { Color = Vector3.One };

		public override bool Metallic { get { return false; } }

		public override Vector3 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                               Vector3 rayWeight, CastRayDelegate castRay,
		                               CancellationToken cancellationToken = default)
		{
			return SampleEmission(intersection.Uv);
		}

		private Vector3 SampleEmission(Vector2 uv)
		{
			if (Emission == null)
				return Vector3.Zero;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			return Emission.Sample(x, y);
		}

		public override Vector3 GetAmbientOcclusion(Scene scene, Random random, Vector3 position, Vector3 normal)
		{
			return Vector3.One;
		}
	}
}
