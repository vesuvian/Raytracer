using System.Drawing;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.Materials
{
	public sealed class Material : AbstractMaterial
	{
		public override Color SampleDiffuse(Vector2 uv)
		{
			if (Diffuse == null)
				return Color;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Color diffuse = Diffuse.Sample(x, y);
			return ColorUtils.Multiply(Color, diffuse);
		}

		public override Vector3 SampleNormal(Vector2 uv)
		{
			if (Normal == null)
				return new Vector3(0, 0, -1);

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Color normal = Normal.Sample(x, y);

			return Vector3.Normalize(new Vector3(normal.R / 255.0f - 0.5f,
			                                     normal.G / 255.0f - 0.5f,
			                                     normal.B / -255.0f - 0.5f) * 2);
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

			Color reflectivity = Reflectivity.Sample(x, y);
			return reflectivity.R / 255.0f;
		}

		public override float SampleRoughness(Vector2 uv)
		{
			if (Reflectivity == null)
				return 0;

			float x = uv.X / Scale.X - Offset.X;
			float y = uv.Y / Scale.Y - Offset.Y;

			Color roughness = Roughness.Sample(x, y);
			return roughness.R / 255.0f;
		}
	}
}
