using System;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects
{
	public sealed class Camera : AbstractSceneObject
	{
		private float m_NearPlane = 0.001f;
		private float m_FarPlane = 1000.0f;
		private float m_Fov = 40.0f;
		private float m_Aspect = 2.0f;
		private Matrix4x4? m_Projection;
		private Matrix4x4? m_ProjectionInverse;

		public float NearPlane
		{
			get { return m_NearPlane; }
			set
			{
				m_NearPlane = value;
				m_Projection = null;
			}
		}

		public float FarPlane
		{
			get { return m_FarPlane; }
			set
			{
				m_FarPlane = value;
				m_Projection = null;
			}
		}

		public float Fov
		{
			get { return m_Fov; }
			set
			{
				m_Fov = value;
				m_Projection = null;
			}
		}

		public float Aspect
		{
			get { return m_Aspect; }
			set
			{
				m_Aspect = value;
				m_Projection = null;
			}
		}

		public float FocalLength { get; set; } = 10.0f;

		public float ApertureSize { get; set; } = 0;

		public int Samples { get; set; } = 1;

		public Matrix4x4 Projection
		{
			get
			{
				if (m_Projection == null)
					m_Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DEG2RAD * Fov, Aspect, NearPlane, FarPlane);
				return m_Projection.Value;
			}
		}

		public Matrix4x4 ProjectionInverse
		{
			get
			{
				if (m_ProjectionInverse == null)
				{
					Matrix4x4 inverse;
					Matrix4x4.Invert(Projection, out inverse);
					m_ProjectionInverse = inverse;
				}
				return m_ProjectionInverse.Value;
			}
		}

		/// <summary>
		/// Creates a camera ray for the given viewport co-ordinates in the range 0 - 1 (bottom left to top right).
		/// </summary>
		/// <param name="minX"></param>
		/// <param name="maxX"></param>
		/// <param name="minY"></param>
		/// <param name="maxY"></param>
		/// <param name="random"></param>
		/// <returns></returns>
		public Ray CreateRay(float minX, float maxX, float minY, float maxY, Random random)
		{
			float x;
			float y;

			if (Samples == 1)
			{
				x = (minX + maxX) / 2;
				y = (minY + maxY) / 2;
			}
			else
			{
				x = random.NextFloat(minX, maxX);
				y = random.NextFloat(minY, maxY);
			}

			// Calculate the local viewport ray
			float rayX = -1 + x * 2;
			float rayY = 1 - y * 2;

			Vector3 direction = Vector3.Normalize(ProjectionInverse.MultiplyPoint(new Vector3(rayX, rayY, 0)));
			direction.Z = -direction.Z;

			// Find the focal point
			Vector3 focalpoint = direction * FocalLength;

			// Offset the start position by a random amount for depth of field
			Vector3 apertureOffset =
				new Vector3(random.NextFloat(-0.5f, 0.5f),
				            random.NextFloat(-0.5f, 0.5f),
				            random.NextFloat(-0.5f, 0.5f)) * ApertureSize;

			// Direction is now the direction from the offset position to the focal point
			Vector3 apertureOffsetDirection = Vector3.Normalize(focalpoint - apertureOffset);

			return new Ray
			{
				Origin = apertureOffset,
				Direction = apertureOffsetDirection
			}.Multiply(LocalToWorld);
		}
	}
}
