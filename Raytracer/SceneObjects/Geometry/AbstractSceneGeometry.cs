using System.Collections.Generic;
using System.Linq;
using Raytracer.Materials;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractSceneGeometry : AbstractSceneObject, ISceneGeometry
	{
		private Aabb? m_Aabb;

		public IMaterial Material { get; set; } = new LambertMaterial();

		public eRayMask RayMask { get; set; } = eRayMask.All;

		public Aabb Aabb { get { return m_Aabb ??= CalculateAabb(); } }

		protected abstract Aabb CalculateAabb();

		public IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			return Aabb.Intersects(ray)
				? GetIntersectionsFinal(ray)
				: Enumerable.Empty<Intersection>();
		}

		protected abstract IEnumerable<Intersection> GetIntersectionsFinal(Ray ray);

		protected override void HandleTransformChange()
		{
			base.HandleTransformChange();

			m_Aabb = null;
		}
	}
}
