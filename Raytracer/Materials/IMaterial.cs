using System.Numerics;
using Raytracer.Materials.Textures;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public interface IMaterial
	{
		Vector4 Color { get; set; }
		Vector2 Scale { get; set; }
		Vector2 Offset { get; set; }
		ITexture Diffuse { get; set; }
		ITexture Emission { get; set; }
		ITexture Normal { get; set; }
		public float NormalScale { get; set; }
		ITexture Reflectivity { get; set; }
		ITexture Roughness { get; set; }

		Vector4 SampleDiffuse(Vector2 uv);
		Vector4 SampleEmission(Vector2 uv);
		Vector3 SampleNormal(Vector2 uv);
		Vector3 GetWorldNormal(Intersection intersection);
		float SampleReflectivity(Vector2 uv);
		float SampleRoughness(Vector2 uv);
	}
}
