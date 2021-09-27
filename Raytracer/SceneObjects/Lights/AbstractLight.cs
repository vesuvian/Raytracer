using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Lights
{
	public abstract class AbstractLight : AbstractSceneObject, ILight
	{
		protected const float SELF_SHADOW_TOLERANCE = 0.0001f;

		public Vector4 Color { get; set; } = ColorUtils.RgbaWhite;
		public bool CastShadows { get; set; } = true;

		public abstract Vector4 Sample(Scene scene, Vector3 position, Vector3 normal, Random random);

		protected Vector4 Shadow(Scene scene, Ray ray, float distance, Vector4 sample)
		{
			if (!CastShadows)
				return sample;

			if (sample == ColorUtils.RgbaBlack)
				return sample;

			KeyValuePair<ISceneGeometry, Intersection> intersection =
				scene.GetIntersections(ray, eRayMask.CastShadows)
					 .Where(kvp => kvp.Value.RayDelta > SELF_SHADOW_TOLERANCE &&
								   kvp.Value.RayDelta < distance - SELF_SHADOW_TOLERANCE)
					 .OrderBy(kvp => kvp.Value.RayDelta)
					 .FirstOrDefault();

			if (intersection.Key == null)
				return sample;

			sample = intersection.Key.Material.Shadow(ray, intersection.Value, sample);

			// Move the ray up to this intersection for the next shadow calculation
			ray.Origin = intersection.Value.Position;
			distance -= intersection.Value.Distance;

			return Shadow(scene, ray, distance, sample);
		}
	}
}
