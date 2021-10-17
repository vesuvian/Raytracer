using System.Collections.Generic;
using System.Linq;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Models
{
	public sealed class ModelSceneGeometry : AbstractSceneGeometry, ISliceableSceneGeometry
	{
		private Mesh m_Mesh;

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

		protected override IEnumerable<Intersection> GetIntersectionsFinal(Ray ray)
		{
			// First transform the ray into local space
			ray = ray.Multiply(WorldToLocal);

            return m_Mesh?.GetIntersections(ray, this, Material)
                         .Select(i => i.Multiply(LocalToWorld)) ?? Enumerable.Empty<Intersection>();
        }

		protected override float CalculateUnscaledSurfaceArea()
        {
            return m_Mesh?.CalculateSurfaceArea() ?? 0;
        }

		protected override Aabb CalculateAabb()
		{
			return m_Mesh?.CalculateAabb(LocalToWorld) ?? default;
		}

        public ISceneGeometry Slice(Aabb aabb)
        {
            ISceneGeometry output = new ModelSliceSceneGeometry(this, aabb);
            return output.SurfaceArea == 0 ? null : output;
        }
    }
}
