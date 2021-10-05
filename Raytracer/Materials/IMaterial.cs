using System;
using System.Numerics;
using System.Threading;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public delegate bool CastRayDelegate(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
	                                     out Vector3 sample, CancellationToken cancellationToken = default);

	public interface IMaterial
	{
		Vector3 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth, Vector3 rayWeight,
		               CastRayDelegate castRay, CancellationToken cancellationToken = default);

		Vector3 Shadow(Ray ray, Intersection intersection, Vector3 light);

		Vector3 GetWorldNormal(Intersection intersection);

		Vector3 GetAmbientOcclusion(Scene scene, Random random, Vector3 position, Vector3 worldNormal);
	}
}
