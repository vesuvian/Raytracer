using System.Linq;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Models
{
	public sealed class ModelSceneGeometry : AbstractSceneGeometry, ISliceableSceneGeometry
	{
		private Mesh m_Mesh;

		public int Complexity { get { return m_Mesh.Triangles.Count; } }

		public Mesh Mesh
		{
			get { return m_Mesh; }
			set
			{
				m_Mesh = value;
				// Force a rebuild of the AABB
				HandleTransformChange();
			}
		}

		protected override bool GetIntersectionFinal(Ray ray, out Intersection intersection, float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity)
		{
			intersection = default;

			if (m_Mesh == null)
				return false;

			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

			float bestT = float.MaxValue;
			bool found = false;
			foreach (Intersection thisIntersection in m_Mesh.GetIntersections(ray, this, Material).Select(i => i.Multiply(LocalToWorld)))
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

		protected override float CalculateUnscaledSurfaceArea()
        {
            return m_Mesh?.CalculateSurfaceArea() ?? 0;
        }

		protected override Aabb CalculateAabb()
		{
			return m_Mesh?.CalculateAabb(LocalToWorld) ?? default;
		}

		public ISliceableSceneGeometry Slice(Aabb aabb)
        {
            return new ModelSliceSceneGeometry(this, aabb);
        }
    }
}
