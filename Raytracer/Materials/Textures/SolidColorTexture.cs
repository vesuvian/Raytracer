using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.Materials.Textures
{
	public sealed class SolidColorTexture : AbstractTexture
	{
		public Vector4 Color { get; set; } = ColorUtils.RgbaWhite;

		public override Vector4 Sample(float u, float v)
		{
			return Color;
		}
	}
}
