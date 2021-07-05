using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class Material : AbstractMaterial
	{
		public override Vector4 SampleDiffuse(Vector2 uv)
		{
			if (Diffuse == null)
				return Color;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 diffuse = Diffuse.Sample(x, y);
			return ColorUtils.Multiply(Color, diffuse);
		}

		public override Vector4 SampleEmission(Vector2 uv)
		{
			if (Emission == null)
				return ColorUtils.RgbaBlack;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			return Emission.Sample(x, y);
		}

		public override Vector3 SampleNormal(Vector2 uv)
		{
			if (Normal == null)
				return new Vector3(0, 0, -1);

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 normal = Normal.Sample(x, y);

			return Vector3.Normalize(new Vector3(normal.X - 0.5f,
			                                     normal.Y - 0.5f,
			                                     -normal.Z - 0.5f) * 2);
		}

		public override Vector3 GetWorldNormal(Intersection intersection)
		{
			// Hack - flip Z and Y since normal map uses Z for "towards"
			Vector3 normalMap = SampleNormal(intersection.Uv);
			normalMap = Vector3.Normalize(new Vector3(normalMap.X * NormalScale,
			                                          normalMap.Z * -1,
			                                          normalMap.Y * NormalScale));

			// Get the normal in world space
			Matrix4x4 surface = Matrix4x4Utils.Tbn(intersection.Tangent, intersection.Bitangent, intersection.Normal);
			return surface.MultiplyNormal(normalMap);
		}

		public override float SampleReflectivity(Vector2 uv)
		{
			if (Reflectivity == null)
				return 0;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 reflectivity = Reflectivity.Sample(x, y);
			return reflectivity.X;
		}

		public override float SampleRoughness(Vector2 uv)
		{
			if (Reflectivity == null)
				return 0;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Vector4 roughness = Roughness.Sample(x, y);
			return roughness.X;
		}
	}
}
