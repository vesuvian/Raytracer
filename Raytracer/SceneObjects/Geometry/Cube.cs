﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class Cube : AbstractSceneGeometry
	{
		public override IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			// First transform ray to local space.
			ray = ray.Multiply(WorldToLocal);

			float[] xt = CheckAxis(ray.Origin.X, ray.Direction.X);
			float[] yt = CheckAxis(ray.Origin.Y, ray.Direction.Y);
			float[] zt = CheckAxis(ray.Origin.Z, ray.Direction.Z);

			float tMin = MathF.Max(MathF.Max(xt[0], yt[0]), zt[0]);
			float tMax = MathF.Min(MathF.Min(xt[1], yt[1]), zt[1]);

			if (tMin > tMax)
				yield break;

			if (tMin > 0)
			{
				Vector3 posMin = ray.PositionAtDelta(tMin);
				Vector3 normalMin = GetNormal(posMin);

				yield return new Intersection
				{
					Normal = normalMin,
					Position = posMin,
					RayOrigin = ray.Origin
				}.Multiply(LocalToWorld);
			}

			if (tMax > 0)
			{
				Vector3 posMax = ray.PositionAtDelta(tMax);
				Vector3 normalMax = GetNormal(posMax);

				yield return new Intersection
				{
					Normal = normalMax,
					Position = posMax,
					RayOrigin = ray.Origin
				}.Multiply(LocalToWorld);
			}
		}

		private static Vector3 GetNormal(Vector3 pos)
		{
			Vector3 cubeMin = Vector3.One * -0.5f;
			Vector3 cubeMax = Vector3.One * 0.5f;
			Vector3 pointToMin = pos - cubeMin;
			Vector3 pointToMax = pos - cubeMax;

			if (MathF.Abs(pointToMin.X) < 0.0001f)
				return new Vector3(-1, 0, 0);
			if (MathF.Abs(pointToMax.X) < 0.0001f)
				return new Vector3(1, 0, 0);
			if (MathF.Abs(pointToMin.Y) < 0.0001f)
				return new Vector3(0, -1, 0);
			if (MathF.Abs(pointToMax.Y) < 0.0001f)
				return new Vector3(0, 1, 0);
			if (MathF.Abs(pointToMin.Z) < 0.0001f)
				return new Vector3(0, 0, -1);
			if (MathF.Abs(pointToMax.Z) < 0.0001f)
				return new Vector3(0, 0, 1);

			return default;
		}

		private static float[] CheckAxis(float origin, float direction)
		{
			float[] t = new float[2];

			float tMinNumerator = (-0.5f - origin);
			float tMaxNumerator = (0.5f - origin);

			//Infinities might pop here due to division by zero
			if (MathF.Abs(direction) >= 0.000001f)
			{
				t[0] = tMinNumerator / direction;
				t[1] = tMaxNumerator / direction;
			}
			else
			{
				t[0] = tMinNumerator * 1e10f;
				t[1] = tMaxNumerator * 1e10f;
			}

			if (t[0] > t[1])
			{
				float temp = t[0];
				t[0] = t[1];
				t[1] = temp;
			}

			return t;
		}
	}
}
