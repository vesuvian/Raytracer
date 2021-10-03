using System;
using System.Numerics;

namespace Raytracer.SceneObjects.Lights
{
	public interface ILight : ISceneObject
	{
		Vector3 Color { get; set; }
		bool CastShadows { get; set; }

		Vector3 Sample(Scene scene, Vector3 position, Vector3 normal, Random random);
	}
}
