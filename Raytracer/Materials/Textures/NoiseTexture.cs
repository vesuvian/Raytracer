using System.Drawing;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class NoiseTexture : AbstractTexture
	{
		private readonly SimplexNoise m_Noise = new SimplexNoise();

		public Color ColorA { get; set; } = Color.White;
		public Color ColorB { get; set; } = Color.Black;
		public int Seed { get { return m_Noise.Seed; } set { m_Noise.Seed = value; } }
		public float Scale { get; set; } = 1;

		public override Color Sample(float u, float v)
		{
			float blend = (m_Noise.Calc(u, v, Scale) + 1) / 2;
			return ColorUtils.Lerp(ColorA, ColorB, blend);
		}
	}
}
