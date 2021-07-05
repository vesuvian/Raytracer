using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class RandomTexture : AbstractTexture
	{
		public Vector4 ColorA { get; set; } = ColorUtils.RgbaWhite;
		public Vector4 ColorB { get; set; } = ColorUtils.RgbaBlack;
		public int Resolution { get; set; } = 2;

		public override Vector4 Sample(float u, float v)
		{
			u = MathF.Round(u, Resolution);
			v = MathF.Round(v, Resolution);

			int seed = HashCode.Combine(u, v);

			Random random = new Random(seed);
			float blend = random.NextFloat();

			return ColorUtils.LerpHsl(ColorA, ColorB, blend);
		}
	}
}
