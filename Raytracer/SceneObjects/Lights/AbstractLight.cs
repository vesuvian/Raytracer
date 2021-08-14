using System;
using System.Numerics;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public abstract class AbstractLight : AbstractSceneObject, ILight
	{
		protected const float SELF_SHADOW_TOLERANCE = 0.0001f;

		public Vector4 Color { get; set; } = ColorUtils.RgbaWhite;
		public bool CastShadows { get; set; } = true;

		public abstract Vector4 Sample(Scene scene, Vector3 position, Vector3 normal, Random random);
	}
}
