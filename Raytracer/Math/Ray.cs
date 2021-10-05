using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Math
{
	public struct Ray
	{
		private Vector3 m_Direction;
		private Vector3 m_InverseDirection;
		private Vector3Int m_Sign;

		public Vector3 Origin { get; set; }

		public Vector3 Direction
		{
			get { return m_Direction; }
			set
			{
				m_Direction = value;

				m_InverseDirection = new Vector3(1 / m_Direction.X,
				                                 1 / m_Direction.Y,
				                                 1 / m_Direction.Z);

				m_Sign = new Vector3Int(m_InverseDirection.X < 0 ? 1 : 0,
				                        m_InverseDirection.Y < 0 ? 1 : 0,
				                        m_InverseDirection.Z < 0 ? 1 : 0);
			}
		}

		public Vector3 InverseDirection { get { return m_InverseDirection; } }

		public Vector3Int Sign { get { return m_Sign; } }

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

		public Ray Reflect(Vector3 position, Vector3 normal)
		{
			return new Ray
			{
				Origin = position,
				Direction = Vector3.Normalize(Vector3.Reflect(Direction, normal))
			};
		}

		public bool Refract(Vector3 position, Vector3 normal, float ior, out Ray ray)
		{
			ray = default;

			Vector3 refracted;
			if (!Vector3Utils.Refract(Direction, normal, ior, out refracted))
				return false;

			ray = new Ray
			{
				Origin = position,
				Direction = refracted
			};

			return true;
		}
	}
}
