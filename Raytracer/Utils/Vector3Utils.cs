using System;
using System.Numerics;

namespace Raytracer.Utils
{
	public static class Vector3Utils
	{
		public static Vector3 Slerp(Vector3 a, Vector3 b, float t)
		{
			float magA = a.Length();
			float magB = b.Length();
			a = Vector3.Normalize(a);
			b = Vector3.Normalize(b);

			float dot = Vector3.Dot(a, b);
			dot = MathF.Max(dot, -1.0f);
			dot = MathF.Min(dot, 1.0f);

			float theta = MathF.Acos(dot) * t;
			Vector3 relativeVec = Vector3.Normalize(b - a * dot);
			Vector3 newVec = a * MathF.Cos(theta) + relativeVec * MathF.Sin(theta);
			return newVec * (magA + (magB - magA) * t);
		}
	}
}
