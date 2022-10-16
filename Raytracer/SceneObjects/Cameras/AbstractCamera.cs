using System;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Cameras
{
	public abstract class AbstractCamera : AbstractSceneObject, ICamera
	{
		private float m_NearPlane = 0.001f;
		private float m_FarPlane = 1000.0f;
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

		public Matrix4x4 Projection
		{
			get
			{
				if (m_Projection == null)
					m_Projection = CalculatePerspective();
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
		public abstract Ray CreateRay(float minX, float maxX, float minY, float maxY, Random random);

		protected abstract Matrix4x4 CalculatePerspective();

		protected void HandleCameraChange()
		{
			m_Projection = null;
			m_ProjectionInverse = null;
		}
	}
}
