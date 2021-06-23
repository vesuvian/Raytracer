using System.Drawing;
using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public abstract class AbstractLight : AbstractSceneObject, ILight
	{
		protected const float SELF_SHADOW_TOLERANCE = 0.0001f;

		public Color Color { get; set; } = Color.White;
		public float Intensity { get; set; } = 1;
		public bool CastShadows { get; set; } = true;

		public abstract Color Sample(Vector3 position, Vector3 normal);
		public abstract bool CanSee(Scene scene, Vector3 position);
	}
}
