using System.Collections.Generic;
using System.Linq;
using Raytracer.Materials;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractSceneGeometry : AbstractSceneObject, ISceneGeometry
	{
		public IMaterial Material { get; set; } = new LambertMaterial();

		public eRayMask RayMask { get; set; } = eRayMask.All;

		public float SurfaceArea { get; private set; }

		public Aabb Aabb { get; private set; }

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

			SurfaceArea = CalculateSurfaceArea();
			Aabb = CalculateAabb();
		}

		private float CalculateSurfaceArea()
		{
			float unscaled = CalculateUnscaledSurfaceArea();
			float scaleFactor = Scale.X * Scale.Y * Scale.Z;
			return unscaled * scaleFactor * scaleFactor;
		}

		protected abstract float CalculateUnscaledSurfaceArea();
	}
}
