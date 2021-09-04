using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Materials;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractSceneGeometry : AbstractSceneObject, ISceneGeometry
	{
		private const float SELF_SHADOW_TOLERANCE = 0.0001f;

		public IMaterial Material { get; set; } = new LambertMaterial();

		public eRayMask RayMask { get; set; } = eRayMask.Default;

		public float SurfaceArea { get; private set; }

		public Aabb Aabb { get; private set; }

		protected abstract Aabb CalculateAabb();

		public IEnumerable<Intersection> GetIntersections(Ray ray)
		{
			return Aabb.Intersects(ray)
				? GetIntersectionsFinal(ray)
				: Enumerable.Empty<Intersection>();
		}

		public abstract Vector3 GetRandomPointOnSurface(Random random = null);

		public Vector4 SampleLight(Scene scene, Vector3 position, Vector3 normal, Random random = null)
		{
			random ??= new Random();

			float faceAmount = Vector3.Dot(normal, Vector3.Normalize(Position - position));
			faceAmount = MathF.Abs(faceAmount);
			faceAmount = MathUtils.Clamp(faceAmount, 0, 1);

			Vector4 sample = new Vector4(0, 0, 50, 1); // TODO

			// Don't bother finding intersections just to draw shadow
			if (sample == ColorUtils.RgbaBlack)
				return ColorUtils.RgbaBlack;

			Vector4 sum = Vector4.Zero;

			for (int i = 0; i < 32; i++)
			{
				Vector3 origin = GetRandomPointOnSurface(random);
				float distance = Vector3.Distance(origin, position);
				float linearFalloff = MathF.Max(10 - distance, 0) / 10;
				Vector4 color = sample * faceAmount * linearFalloff;

				Ray ray =
					new Ray
					{
						Origin = origin,
						Direction = Vector3.Normalize(position - origin)
					};

				bool canSee =
					!scene.GetIntersections(ray, eRayMask.CastShadows)
					      .Where(kvp => kvp.Key != this)
					      .Any(kvp => kvp.Value.RayDelta > SELF_SHADOW_TOLERANCE &&
					                  kvp.Value.RayDelta < distance);

				if (canSee)
					sum += color;
			}

			return sum / 4;
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
