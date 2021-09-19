﻿using System;
using System.Numerics;
using System.Threading;
using Raytracer.Math;

namespace Raytracer.Materials
{
	public delegate Vector4 CastRayDelegate(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
	                                        out bool hit, CancellationToken cancellationToken = default);

	public interface IMaterial
	{
		Vector4 Sample(Scene scene, Ray ray, Intersection intersection, Random random, int rayDepth, Vector3 rayWeight,
		               CastRayDelegate castRay, CancellationToken cancellationToken = default);

		Vector4 Shadow(Ray ray, Intersection intersection, Vector4 light);

		Vector3 GetWorldNormal(Intersection intersection);

		Vector4 GetAmbientOcclusion(Scene scene, Random random, Vector3 position, Vector3 worldNormal);
	}
}
