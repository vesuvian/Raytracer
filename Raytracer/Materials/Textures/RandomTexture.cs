using System;
using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Materials.Textures
{
	public sealed class RandomTexture : AbstractTexture
	{
		public Vector3 ColorA { get; set; } = Vector3.One;
		public Vector3 ColorB { get; set; } = Vector3.Zero;
		public int Resolution { get; set; } = 2;

		public override Vector3 Sample(float u, float v)
		{
			u = MathF.Round(u, Resolution);
			v = MathF.Round(v, Resolution);

			int seed = HashCode.Combine(u, v);

			Random random = new Random(seed);
			float blend = random.NextFloat();

			Vector3 aHsl = ColorA.FromRgbToHsl();
			Vector3 bHsl = ColorB.FromRgbToHsl();
			Vector3 lerpHsl = Vector3.Lerp(aHsl, bHsl, blend);

			return lerpHsl.FromHslToRgb();
		}
	}
}
