using System;
using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class AmbientLight : AbstractLight
	{
		public override Vector4 Sample(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			return Color;
		}
	}
}
