using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.Extensions
{
	public static class Vector3Extensions
	{
		public static Vector4 ToVector4(this Vector3 extends, float w = 0)
		{
			return new Vector4(extends.X, extends.Y, extends.Z, w);
		}

		public static Vector3 Sum(this IEnumerable<Vector3> extends)
		{
			return extends.Aggregate((a, b) => a + b);
		}

        public static float GetValue(this Vector3 extends, eAxis axis)
        {
            return axis switch
            {
                eAxis.X => extends.X,
                eAxis.Y => extends.Y,
                eAxis.Z => extends.Z,
                _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
            };
        }

        public static Vector3 SetValue(this Vector3 extends, eAxis axis, float value)
        {
            switch (axis)
            {
                case eAxis.X:
                    extends.X = value;
                    break;
                case eAxis.Y:
                    extends.Y = value;
                    break;
                case eAxis.Z:
                    extends.Z = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
            }

            return extends;
        }
    }
}
