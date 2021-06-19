using System.Numerics;
using Raytracer.Extensions;

namespace Raytracer.SceneObjects
{
	public abstract class AbstractSceneObject : ISceneObject
	{
		private Vector3 m_Position;
		private Vector3 m_Scale;
		private Quaternion m_Rotation;
		private Matrix4x4? m_LocalToWorld;
		private Matrix4x4? m_WorldToLocal;

		public Vector3 Position
		{
			get { return m_Position; }
			set
			{
				m_Position = value;
				m_LocalToWorld = null;
				m_WorldToLocal = null;
			}
		}

		public Vector3 Scale
		{
			get { return m_Scale; }
			set
			{
				m_Scale = value;
				m_LocalToWorld = null;
				m_WorldToLocal = null;
			}
		}

		public Quaternion Rotation
		{
			get { return m_Rotation; }
			set
			{
				m_Rotation = value;
				m_LocalToWorld = null;
				m_WorldToLocal = null;
			}
		}

		public Matrix4x4 LocalToWorld
		{
			get
			{
				if (m_LocalToWorld == null)
				{
					m_LocalToWorld = Matrix4x4.CreateTranslation(Position) *
					                 Matrix4x4.CreateFromQuaternion(Rotation) *
					                 Matrix4x4.CreateScale(Scale);
				}
				return m_LocalToWorld.Value;
			}
		}

		public Matrix4x4 WorldToLocal
		{
			get
			{
				if (m_WorldToLocal == null)
				{
					Matrix4x4 inverse;
					Matrix4x4.Invert(LocalToWorld, out inverse);
					m_WorldToLocal = inverse;
				}
				return m_WorldToLocal.Value;
			}
		}

		public Vector3 Forward { get { return LocalToWorld.MultiplyNormal(new Vector3(0, 0, 1)); } }

		protected AbstractSceneObject()
		{
			Position = new Vector3(0, 0, 0);
			Scale = new Vector3(1, 1, 1);
			Rotation = Quaternion.Identity;
		}
	}
}
