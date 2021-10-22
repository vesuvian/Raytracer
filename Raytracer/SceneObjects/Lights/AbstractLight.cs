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

			while (true)
			{
				if (sample == Vector3.Zero)
					return sample;

				Intersection intersection;
				if (!scene.GetIntersection(ray, out intersection, eRayMask.CastShadows, SELF_SHADOW_TOLERANCE, distance - SELF_SHADOW_TOLERANCE))
					return sample;

				sample = intersection.Material.Shadow(ray, intersection, sample);

				// Move the ray up to this intersection for the next shadow calculation
				ray.Origin = intersection.Position;
				distance -= intersection.Distance;
			}
		}
	}
}
