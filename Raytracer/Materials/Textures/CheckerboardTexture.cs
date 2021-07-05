using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class CheckerboardTexture : AbstractTexture
	{
		public Vector4 ColorA { get; set; } = ColorUtils.RgbaWhite;
		public Vector4 ColorB { get; set; } = ColorUtils.RgbaBlack;
		public Vector2 Periodicity { get; set; } = new Vector2(4);

		public override Vector4 Sample(float u, float v)
		{
			bool a = (MathUtils.ModPositive(u * (Periodicity.X / 2), 1) < 0.5f) ^
			         (MathUtils.ModPositive(v * (Periodicity.Y / 2), 1) < 0.5f);

			return a ? ColorA : ColorB;
		}
	}
}
