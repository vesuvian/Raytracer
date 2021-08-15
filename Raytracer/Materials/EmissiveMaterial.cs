using System;
using System.Numerics;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class EmissiveMaterial : AbstractMaterial
	{
		public ITexture Emission { get; set; } = new SolidColorTexture { Color = ColorUtils.RgbaWhite };

		public override Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth, CastRayDelegate castRay)
		{
			return SampleEmission(intersection.Uv);
		}

		private Vector4 SampleEmission(Vector2 uv)
		{
			if (Emission == null)
				return ColorUtils.RgbaBlack;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			return Emission.Sample(x, y);
		}
	}
}
