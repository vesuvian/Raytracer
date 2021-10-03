using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.Materials.Textures
{
	public sealed class NoiseTexture : AbstractTexture
	{
		private readonly SimplexNoise m_Noise = new SimplexNoise();

		public Vector3 ColorA { get; set; } = Vector3.One;
		public Vector3 ColorB { get; set; } = Vector3.Zero;
		public int Seed { get { return m_Noise.Seed; } set { m_Noise.Seed = value; } }
		public float Scale { get; set; } = 1;

		public override Vector3 Sample(float u, float v)
		{
			float blend = (m_Noise.Calc(u, v, Scale) + 1) / 2;

			Vector3 hslA = ColorA.FromRgbToHsl();
			Vector3 hslB = ColorB.FromRgbToHsl();
			Vector3 hslLerp = Vector3.Lerp(hslA, hslB, blend);

			return hslLerp.FromHslToRgb();
		}
	}
}
