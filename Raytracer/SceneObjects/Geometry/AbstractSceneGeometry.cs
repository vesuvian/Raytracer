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

		public IEnumerable<Intersection> GetIntersections(Ray ray, eRayMask mask, float minDelta = float.NegativeInfinity,
                                                          float maxDelta = float.PositiveInfinity)
		{
			if ((RayMask & mask) == eRayMask.None)
				return Enumerable.Empty<Intersection>();

            float tMin;
            float tMax;
			if (!Aabb.Intersects(ray, out tMin, out tMax))
                return Enumerable.Empty<Intersection>();

            if ((tMin < minDelta && tMax < minDelta) ||
                (tMin > maxDelta && tMax > maxDelta))
                return Enumerable.Empty<Intersection>();

			return GetIntersectionsFinal(ray).Where(i => i.RayDelta >= minDelta && i.RayDelta <= maxDelta);
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
