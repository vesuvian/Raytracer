using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class CheckerboardTexture : AbstractTexture
	{
		public Color ColorA { get; set; } = Color.White;
		public Color ColorB { get; set; } = Color.Black;
		public Vector2 Periodicity { get; set; } = new Vector2(4);

		public override Color Sample(float u, float v)
		{
			bool a = (MathUtils.ModPositive(u * (Periodicity.X / 2), 1) < 0.5f) ^
			         (MathUtils.ModPositive(v * (Periodicity.Y / 2), 1) < 0.5f);

			return a ? ColorA : ColorB;
		}
	}
}
