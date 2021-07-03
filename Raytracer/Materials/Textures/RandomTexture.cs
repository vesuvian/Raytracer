using System;
using System.Drawing;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class RandomTexture : AbstractTexture
	{
		public Color ColorA { get; set; } = Color.White;
		public Color ColorB { get; set; } = Color.Black;
		public int Resolution { get; set; } = 2;

		public override Color Sample(float u, float v)
		{
			u = MathF.Round(u, Resolution);
			v = MathF.Round(v, Resolution);

			int seed = HashCode.Combine(u, v);

			Random random = new Random(seed);
			float blend = random.NextFloat();

			return ColorUtils.Lerp(ColorA, ColorB, blend);
		}
	}
}
