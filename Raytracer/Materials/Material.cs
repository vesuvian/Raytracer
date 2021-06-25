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

			lock (Diffuse)
			{
				int x = (int)(Diffuse.Width * uv.X * Scale.X);
				int y = (int)(Diffuse.Height * uv.Y * Scale.Y);

				x = MathUtils.ModPositive(x, Diffuse.Width);
				y = MathUtils.ModPositive(y, Diffuse.Height);

				Color diffuse = Diffuse.GetPixel(x, y);
				return ColorUtils.Multiply(Color, diffuse);
			}
		}
	}
}
