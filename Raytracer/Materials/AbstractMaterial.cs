using System.Drawing;
using System.Numerics;
using Raytracer.Materials.Textures;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Color Color { get; set; } = Color.Gray;
		public ITexture Diffuse { get; set; } = new SolidColorTexture {Color = Color.White};
		public ITexture Emission { get; set; } = new SolidColorTexture {Color = Color.Black};
		public ITexture Normal { get; set; } = new SolidColorTexture {Color = Color.FromArgb(128, 128, 255)};
		public float NormalScale { get; set; } = 1.0f;
		public ITexture Reflectivity { get; set; } = new SolidColorTexture {Color = Color.Black};
		public ITexture Roughness { get; set; } = new SolidColorTexture {Color = Color.Black};
		public Vector2 Scale { get; set; } = Vector2.One;
		public Vector2 Offset { get; set; }

		public abstract Color SampleDiffuse(Vector2 uv);
		public abstract Color SampleEmission(Vector2 uv);
		public abstract Vector3 SampleNormal(Vector2 uv);
		public abstract Vector3 GetWorldNormal(Intersection intersection);
		public abstract float SampleReflectivity(Vector2 uv);
		public abstract float SampleRoughness(Vector2 uv);
	}
}
