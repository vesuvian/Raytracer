using System.Drawing;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class Material : AbstractMaterial
	{
		public override Color Sample(Vector2 uv)
		{
			if (Diffuse == null)
				return Color;

			float x = uv.X * Scale.Y;
			float y = uv.Y * Scale.Y;

			Color diffuse = Diffuse.Sample(x, y);
			return ColorUtils.Multiply(Color, diffuse);
		}
	}
}
