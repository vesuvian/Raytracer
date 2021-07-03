using System.Drawing;

namespace Raytracer.Materials.Textures
{
	public sealed class SolidColorTexture : AbstractTexture
	{
		public Color Color { get; set; } = Color.White;

		public override Color Sample(float u, float v)
		{
			return Color;
		}
	}
}
