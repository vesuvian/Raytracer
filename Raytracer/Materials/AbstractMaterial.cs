using System.Drawing;
using System.Numerics;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Color Color { get; set; } = Color.Gray;
		public Texture Diffuse { get; set; }
		public Texture Normal { get; set; }
		public float NormalIntensity { get; set; } = 1.0f;
		public Vector2 Scale { get; set; } = Vector2.One;

		public abstract Color Sample(Vector2 uv);
		public abstract Vector3 SampleNormal(Vector2 uv);
	}
}
