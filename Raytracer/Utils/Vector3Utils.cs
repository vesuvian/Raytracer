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

		public static Vector3 Refract(Vector3 direction, Vector3 normal, float ior)
		{
			float cosi = MathUtils.Clamp(-1, 1, Vector3.Dot(direction, normal));
			float etai = 1, etat = ior;
			Vector3 n = normal;

			if (cosi < 0)
				cosi = -cosi;
			else
			{
				float temp = etai;
				etai = etat;
				etat = temp;
				n = -n;
			}

			float eta = etai / etat;
			float k = 1 - eta * eta * (1 - cosi * cosi);
			return k < 0
				? Vector3.Zero
				: eta * direction + (eta * cosi - MathF.Sqrt(k)) * n;
		}
	}
}
