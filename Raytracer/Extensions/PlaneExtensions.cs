using System;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.Extensions
{
	public static class PlaneExtensions
	{
		public static bool IsInFront(this Plane extends, Vector3 point)
		{
			return Vector3.Dot(extends.Normal, point) - extends.D > 0.0f;
		}

		public static bool IsBehind(this Plane extends, Vector3 point)
		{
			return Vector3.Dot(extends.Normal, point) - extends.D < 0.0f;
		}

		public static float Distance(this Plane extends, Vector3 point)
		{
			return MathF.Abs(Vector3.Dot(extends.Normal, point) - extends.D);
		}

        public static bool GetIntersection(this Plane extends, Ray ray, out float t)
        {
            t = default;

            // Line and plane are parallel
            float dotDenominator = Vector3.Dot(ray.Direction, extends.Normal);
            if (System.Math.Abs(dotDenominator) < 0.00001f)
                return false;

            float dotNumerator = -extends.Distance(ray.Origin);
            float length = dotNumerator / dotDenominator;

            if (System.Math.Abs(length) > 0.00001f)
                t = length;

            return true;
        }
	}
}
