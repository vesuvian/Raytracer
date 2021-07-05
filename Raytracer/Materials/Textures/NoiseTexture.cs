using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class NoiseTexture : AbstractTexture
	{
		private readonly SimplexNoise m_Noise = new SimplexNoise();

		public Vector4 ColorA { get; set; } = ColorUtils.RgbaWhite;
		public Vector4 ColorB { get; set; } = ColorUtils.RgbaBlack;
		public int Seed { get { return m_Noise.Seed; } set { m_Noise.Seed = value; } }
		public float Scale { get; set; } = 1;

		public override Vector4 Sample(float u, float v)
		{
			float blend = (m_Noise.Calc(u, v, Scale) + 1) / 2;
			return ColorUtils.LerpHsl(ColorA, ColorB, blend);
		}
	}
}
