using System.Numerics;

namespace Raytracer.Materials.Textures
{
	public sealed class SolidColorTexture : AbstractTexture
	{
		public Vector3 Color { get; set; } = Vector3.One;

		public override Vector3 Sample(float u, float v)
		{
			return Color;
		}
	}
}
