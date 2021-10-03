using System;
using System.Linq;
using System.Numerics;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Lights
{
	public abstract class AbstractLight : AbstractSceneObject, ILight
	{
		protected const float SELF_SHADOW_TOLERANCE = 0.0001f;

		public Vector3 Color { get; set; } = Vector3.One;
		public bool CastShadows { get; set; } = true;

		public abstract Vector3 Sample(Scene scene, Vector3 position, Vector3 normal, Random random);

		protected Vector3 Shadow(Scene scene, Ray ray, float distance, Vector3 sample)
		{
			if (!CastShadows)
				return sample;

			if (sample == Vector3.Zero)
				return sample;

			var (geometry, intersection) =
				scene.GetIntersections(ray, eRayMask.CastShadows)
				     .Where(kvp => kvp.Value.RayDelta > SELF_SHADOW_TOLERANCE &&
				                   kvp.Value.RayDelta < distance - SELF_SHADOW_TOLERANCE)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return sample;

			sample = geometry.Material.Shadow(ray, intersection, sample);

			// Move the ray up to this intersection for the next shadow calculation
			ray.Origin = intersection.Position;
			distance -= intersection.Distance;

			return Shadow(scene, ray, distance, sample);
		}
	}
}
