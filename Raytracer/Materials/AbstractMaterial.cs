using System.Numerics;
using Raytracer.Materials.Textures;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Vector4 Color { get; set; } = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
		public ITexture Diffuse { get; set; } = new SolidColorTexture {Color = ColorUtils.RgbaWhite};
		public ITexture Emission { get; set; } = new SolidColorTexture {Color = ColorUtils.RgbaBlack};
		public ITexture Normal { get; set; } = new SolidColorTexture {Color = new Vector4(0.5f, 0.5f, 1.0f, 1.0f)};
		public float NormalScale { get; set; } = 1.0f;
		public ITexture Reflectivity { get; set; } = new SolidColorTexture {Color = ColorUtils.RgbaBlack};
		public ITexture Roughness { get; set; } = new SolidColorTexture {Color = ColorUtils.RgbaBlack};
		public Vector2 Scale { get; set; } = Vector2.One;
		public Vector2 Offset { get; set; }

		public abstract Vector4 SampleDiffuse(Vector2 uv);
		public abstract Vector4 SampleEmission(Vector2 uv);
		public abstract Vector3 SampleNormal(Vector2 uv);
		public abstract Vector3 GetWorldNormal(Intersection intersection);
		public abstract float SampleReflectivity(Vector2 uv);
		public abstract float SampleRoughness(Vector2 uv);
	}
}
