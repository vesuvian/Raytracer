using System.Diagnostics;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Math
{
    [DebuggerDisplay("Origin = {Origin}, Direction = {Direction}")]
	public struct Ray
    {
        public readonly Vector3 Origin;
        public readonly Vector3 Direction;

        public Vector3 InverseDirection
        {
            get
            {
                return new Vector3(1 / Direction.X,
                                   1 / Direction.Y,
                                   1 / Direction.Z);
            }
        }

        public Vector3Int Sign
        {
            get
            {
                return new Vector3Int(InverseDirection.X < 0 ? 1 : 0,
                                      InverseDirection.Y < 0 ? 1 : 0,
                                      InverseDirection.Z < 0 ? 1 : 0);
            }
        }

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="origin"></param>
		/// <param name="direction"></param>
        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
		}

		public Ray Multiply(Matrix4x4 matrix)
        {
            var origin = matrix.MultiplyPoint(Origin);
            var direction = Vector3.Normalize(matrix.MultiplyDirection(Direction));
            return new Ray(origin, direction);
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

		public Ray Reflect(Vector3 position, Vector3 normal)
		{
            var direction = Vector3.Normalize(Vector3.Reflect(Direction, normal));
            return new Ray(position, direction);
        }

		public bool Refract(Vector3 position, Vector3 normal, float ior, out Ray ray)
		{
			ray = default;

			Vector3 refracted;
			if (!Vector3Utils.Refract(Direction, normal, ior, out refracted))
				return false;

            ray = new Ray(position, refracted);
            return true;
		}
	}
}
