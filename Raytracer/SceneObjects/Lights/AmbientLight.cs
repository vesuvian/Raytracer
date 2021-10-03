using System;
using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public sealed class AmbientLight : AbstractLight
	{
		public override Vector3 Sample(Scene scene, Vector3 position, Vector3 normal, Random random)
		{
			return Color;
		}
	}
}
