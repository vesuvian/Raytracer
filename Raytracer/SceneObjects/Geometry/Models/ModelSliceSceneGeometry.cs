using System;
using System.Numerics;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Models
{
	public sealed class ModelSliceSceneGeometry : ISliceableSceneGeometry
	{
		private readonly Mesh m_Mesh;
		private readonly ModelSceneGeometry m_Model;

		public int Complexity { get { return m_Mesh.Triangles.Count; } }

		public Mesh Mesh { get { return m_Mesh; } }

		Vector3 ISceneObject.Position { get => Vector3.Zero; set => throw new NotSupportedException(); }

		Vector3 ISceneObject.Scale { get => Vector3.One; set => throw new NotSupportedException(); }

		Quaternion ISceneObject.Rotation { get => Quaternion.Identity; set => throw new NotSupportedException(); }

		Matrix4x4 ISceneObject.LocalToWorld => Matrix4x4.Identity;

		Matrix4x4 ISceneObject.WorldToLocal => Matrix4x4.Identity;

		Vector3 ISceneObject.Forward => Vector3.UnitZ;

		eRayMask ISceneGeometry.RayMask => m_Model.RayMask;

		public float SurfaceArea { get; }

		public Aabb Aabb { get; }

		public bool GetIntersection(Ray ray, eRayMask mask, out Intersection intersection,
		                            float minDelta = float.NegativeInfinity,
		                            float maxDelta = float.PositiveInfinity, bool testAabb = true)
		{
			intersection = default;

			if (m_Mesh == null)
				return false;

			if ((m_Model.RayMask & mask) == eRayMask.None)
				return false;

			if (testAabb)
			{
				if (!Aabb.Intersects(ray, minDelta, maxDelta))
					return false;
			}

			float bestT = float.MaxValue;
			bool found = false;
			foreach (Intersection thisIntersection in m_Mesh.GetIntersections(ray, this, m_Model.Material))
			{
				float t = thisIntersection.RayDelta;

				if (t < minDelta || t > maxDelta)
					continue;

				if (t > bestT)
					continue;

				bestT = thisIntersection.RayDelta;
				found = true;
				intersection = thisIntersection;
			}

			return found;
		}

		public ISliceableSceneGeometry Slice(Aabb aabb)
		{
			return m_Model.Slice(aabb);
		}

		public ModelSliceSceneGeometry(ModelSceneGeometry model, Aabb aabb)
		{
			m_Model = model;
			m_Mesh = model.Mesh.Clip(model.LocalToWorld, aabb);

			SurfaceArea = m_Mesh.CalculateSurfaceArea();
			Aabb = m_Mesh.CalculateAabb(Matrix4x4.Identity);
		}
	}
}
