using System;
using System.Numerics;
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
		                      CastRayDelegate castRay)
		{
			float blend = SampleBlend(intersection.Uv);

			Vector4 a = blend < 1 ? A.Sample(scene, ray, intersection, random, rayDepth, castRay) : Vector4.Zero;
			Vector4 b = blend > 0 ? B.Sample(scene, ray, intersection, random, rayDepth, castRay) : Vector4.Zero;

			return ColorUtils.LerpRgb(a, b, blend);
		}

		public Vector3 GetWorldNormal(Intersection intersection)
		{
			float blend = SampleBlend(intersection.Uv);

			Vector3 a = A.GetWorldNormal(intersection);
			Vector3 b = B.GetWorldNormal(intersection);

			return Vector3Utils.Slerp(a, b, blend);
		}

		private float SampleBlend(Vector2 uv)
		{
			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			return Blend.Sample(x, y).X;
		}
	}
}
