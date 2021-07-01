using System.Drawing;
using System.Numerics;

namespace Raytracer.Materials
{
	public interface IMaterial
	{
		Color Color { get; set; }
		Vector2 Scale { get; set; }
		Texture Diffuse { get; set; }
		Texture Normal { get; set; }
		public float NormalIntensity { get; set; }

		Color Sample(Vector2 uv);
		Vector3 SampleNormal(Vector2 uv);
	}
}
