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
			float scale = (float)System.Math.Tan(MathUtils.DEG2RAD * Fov * 0.5f);

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
			float rayX = (2 * x - 1) * Aspect * scale;
			float rayY = (1 - 2 * y) * scale;
			Vector3 direction = Vector3.Normalize(new Vector3(rayX, rayY, 1));

			// Find the focal point
			Vector3 focalpoint = new Ray {Direction = direction}.PositionAtDelta(FocalLength);

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
