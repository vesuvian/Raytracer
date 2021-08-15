using System;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public delegate Vector4 CastRayDelegate(Scene scene, Ray ray, Random random, int rayDepth);

	public interface IMaterial
	{
		Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth, CastRayDelegate castRay);

		Vector3 GetWorldNormal(Intersection intersection);
	}
}
