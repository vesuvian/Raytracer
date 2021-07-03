using System.Drawing;
using System.Numerics;
using Raytracer.Materials.Textures;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Color Color { get; set; } = Color.Gray;
		public ITexture Diffuse { get; set; }
		public ITexture Normal { get; set; }
		public float NormalScale { get; set; } = 1.0f;
		public Vector2 Scale { get; set; } = Vector2.One;
		public Vector2 Offset { get; set; }

		public abstract Color Sample(Vector2 uv);
		public abstract Vector3 SampleNormal(Vector2 uv);
	}
}
