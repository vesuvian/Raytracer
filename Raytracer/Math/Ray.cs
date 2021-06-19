using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.Math
{
	public struct Ray
	{
		public Vector3 Origin { get; set; }
		public Vector3 Direction { get; set; }

		public Ray Multiply(Matrix4x4 matrix)
		{
			return new Ray
			{
				Origin = matrix.MultiplyPoint(Origin),
				Direction = Vector3.Normalize(matrix.MultiplyDirection(Direction))
			};
		}

		/// <summary>
		/// Gets the position at the delta along the ray.
		/// </summary>
		/// <param name="delta"></param>
		/// <returns></returns>
		public Vector3 PositionAtDelta(float delta)
		{
			return Origin + delta * Direction;
		}
	}
}
