using Raytracer.Geometry;
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

		public bool GetIntersection(Ray ray, eRayMask mask, out Intersection intersection,
		                            float minDelta = float.NegativeInfinity,
		                            float maxDelta = float.PositiveInfinity, bool testAabb = true)
		{
			intersection = default;

			if ((RayMask & mask) == eRayMask.None)
				return false;

			if (testAabb)
			{
				float tMin;
				float tMax;
				if (!Aabb.Intersects(ray, out tMin, out tMax))
					return false;

				if ((tMin < minDelta && tMax < minDelta) ||
				    (tMin > maxDelta && tMax > maxDelta))
					return false;
			}

			return GetIntersectionFinal(ray, out intersection, minDelta, maxDelta);
		}

		protected abstract Aabb CalculateAabb();

		protected abstract bool GetIntersectionFinal(Ray ray, out Intersection intersection,
		                                             float minDelta = float.NegativeInfinity,
		                                             float maxDelta = float.PositiveInfinity);

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
