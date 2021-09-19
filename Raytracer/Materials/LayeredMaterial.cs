using System;
using System.Numerics;
using System.Threading;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class LayeredMaterial : IMaterial
	{
		public Vector2 Scale { get; set; } = Vector2.One;
		public Vector2 Offset { get; set; }
		public ITexture Blend { get; set; } = new SolidColorTexture { Color = ColorUtils.RgbaBlack };

		public IMaterial A { get; set; } = new LambertMaterial();
		public IMaterial B { get; set; } = new LambertMaterial();

		public Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth,
		                      Vector3 rayWeight, CastRayDelegate castRay, CancellationToken cancellationToken = default)
		{
			float blend = SampleBlend(intersection.Uv);

			Vector4 a = blend < 1 ? A.Sample(scene, ray, intersection, random, rayDepth, rayWeight * (1 - blend), castRay, cancellationToken) : Vector4.Zero;
			Vector4 b = blend > 0 ? B.Sample(scene, ray, intersection, random, rayDepth, rayWeight * blend, castRay, cancellationToken) : Vector4.Zero;

			return ColorUtils.LerpRgb(a, b, blend);
		}

		public Vector4 Shadow(Ray ray, Intersection intersection, Vector4 light)
		{
			float blend = SampleBlend(intersection.Uv);

			Vector4 a = A.Shadow(ray, intersection, light);
			Vector4 b = B.Shadow(ray, intersection, light);

			return Vector4.Lerp(a, b, blend);
		}

		public Vector3 GetWorldNormal(Intersection intersection)
		{
			float blend = SampleBlend(intersection.Uv);

			Vector3 a = A.GetWorldNormal(intersection);
			Vector3 b = B.GetWorldNormal(intersection);

			return Vector3Utils.Slerp(a, b, blend);
		}

		public Vector4 GetAmbientOcclusion(Scene scene, Random random, Vector3 position, Vector3 worldNormal)
		{
			Vector4 a = A.GetAmbientOcclusion(scene, random, position, worldNormal);
			Vector4 b = B.GetAmbientOcclusion(scene, random, position, worldNormal);

			return Vector4.Min(a, b);
		}

		private float SampleBlend(Vector2 uv)
		{
			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			return Blend.Sample(x, y).X;
		}
	}
}
