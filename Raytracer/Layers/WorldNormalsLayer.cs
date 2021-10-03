using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Layers
{
	public sealed class WorldNormalsLayer : AbstractLayer
	{
		protected override Vector3 CastRay(Scene scene, Ray ray, Random random, int rayDepth, Vector3 rayWeight,
		                                   out bool hit, CancellationToken cancellationToken = default)
		{
			cancellationToken.ThrowIfCancellationRequested();

			hit = false;

			(ISceneGeometry geometry, Intersection intersection) =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .Where(kvp => kvp.Value.RayDelta > 0.00001f)
				     .OrderBy(kvp => kvp.Value.RayDelta)
				     .FirstOrDefault();

			if (geometry == null)
				return Vector3.Zero;
			hit = true;

			Vector3 worldNormal = geometry.Material.GetWorldNormal(intersection);
			return (worldNormal / 2) + (Vector3.One / 2);
		}
	}
}
