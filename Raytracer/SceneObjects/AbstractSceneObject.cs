using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.SceneObjects
{
	public abstract class AbstractSceneObject : ISceneObject
	{
		private Vector3 m_Position;
		private Vector3 m_Scale = Vector3.One;
		private Quaternion m_Rotation;
		private Matrix4x4? m_LocalToWorld;
		private Matrix4x4? m_WorldToLocal;

		public Vector3 Position
		{
			get { return m_Position; }
			set
			{
				m_Position = value;
				HandleTransformChange();
			}
		}

		public Vector3 Scale
		{
			get { return m_Scale; }
			set
			{
				m_Scale = value;
				HandleTransformChange();
			}
		}

		public Quaternion Rotation
		{
			get { return m_Rotation; }
			set
			{
				m_Rotation = value;
				HandleTransformChange();
			}
		}

		public Matrix4x4 LocalToWorld
		{
			get { return m_LocalToWorld ??= Matrix4x4Utils.Trs(Position, Rotation, Scale); }
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

		protected virtual void HandleTransformChange()
		{
			m_LocalToWorld = null;
			m_WorldToLocal = null;
		}
	}
}
