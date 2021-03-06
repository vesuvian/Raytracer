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

			float dot = MathUtils.Clamp(Vector3.Dot(a, b), -1, 1);
			float theta = MathF.Acos(dot) * t;
			Vector3 relativeVec = b - a * dot;
			relativeVec = relativeVec == Vector3.Zero ? Vector3.Zero : Vector3.Normalize(relativeVec);
			Vector3 newVec = a * MathF.Cos(theta) + relativeVec * MathF.Sin(theta);
			return newVec * (magA + (magB - magA) * t);
		}

		public static Tuple<Vector3, Vector3> GetTangentAndBitangent(Vector3 normal)
		{
			Vector3 tangent;
			if (MathF.Abs(normal.X) > MathF.Abs(normal.Y)) 
				tangent = new Vector3(normal.Z, 0, -normal.X) / MathF.Sqrt(normal.X* normal.X + normal.Z* normal.Z); 
			else
				tangent = new Vector3(0, -normal.Z, normal.Y) / MathF.Sqrt(normal.Y* normal.Y + normal.Z* normal.Z);

			Vector3 bitangent = Vector3.Cross(normal, tangent);

			return new Tuple<Vector3, Vector3>(tangent, bitangent);
		}

		public static bool Refract(Vector3 direction, Vector3 normal, float ior, out Vector3 output)
		{
			output = Vector3.Zero;

			float faceAmount = MathUtils.Clamp(Vector3.Dot(direction, normal), - 1, 1);
			float fromIor = 1;
			float toIor = ior;

			if (faceAmount < 0)
				faceAmount = -faceAmount;
			else
			{
				(fromIor, toIor) = (toIor, fromIor);
				normal = -normal;
			}

			float ratio = fromIor / toIor;
			float k = 1 - ratio * ratio * (1 - faceAmount * faceAmount);

			if (k < 0)
				return false;

			output = Vector3.Normalize(direction * ratio + normal * (ratio * faceAmount - MathF.Sqrt(k)));
			return true;
		}

		public static float Fresnel(Vector3 direction, Vector3 normal, float ior)
		{
			float cosi = MathUtils.Clamp(Vector3.Dot(direction, normal), -1, 1);
			float etai = 1;
			float etat = ior;

			if (cosi > 0)
				(etai, etat) = (etat, etai);

			// Compute sini using Snell's law
			float sint = etai / etat * MathF.Sqrt(MathF.Max(0.0f, 1 - cosi * cosi));
			if (sint >= 1)
				return 1; // Total internal reflection

			float cost = MathF.Sqrt(MathF.Max(0.0f, 1 - sint * sint));
			cosi = MathF.Abs(cosi);
			float rs = ((etat * cosi) - (etai * cost)) / ((etat * cosi) + (etai * cost));
			float rp = ((etai * cosi) - (etat * cost)) / ((etai * cosi) + (etat * cost));
			return (rs * rs + rp * rp) / 2;
		}
	}
}
