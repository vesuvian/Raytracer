using System.Drawing;
using System.Numerics;

namespace Raytracer.Materials
{
	public abstract class AbstractMaterial : IMaterial
	{
		public Color Color { get; set; } = Color.Gray;
		public Bitmap Diffuse { get; set; }
		public Vector2 Scale { get; set; } = Vector2.One;

		public abstract Color Sample(Vector2 uv);
	}
}
