using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Materials;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public interface ISceneGeometry : ISceneObject
	{
		IMaterial Material { get; set; }

		eRayMask RayMask { get; set; }

		float SurfaceArea { get; }

		Aabb Aabb { get; }

		IEnumerable<Intersection> GetIntersections(Ray ray);

		Vector3 GetRandomPointOnSurface(Random random = null);

		Vector4 SampleLight(Scene scene, Vector3 position, Vector3 normal, Random random = null);
	}
}
