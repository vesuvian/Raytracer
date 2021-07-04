using System.Drawing;
using System.Linq;
using System.Numerics;
using Raytracer.Math;
using Raytracer.SceneObjects;

namespace Raytracer.Layers
{
	public sealed class WorldPositionLayer : AbstractLayer
	{
		public Vector3 Min { get; set; } = Vector3.One * -10;
		public Vector3 Max { get; set; } = Vector3.One * 10;

		protected override Color CastRay(Scene scene, Ray ray, int rayDepth)
		{
			Intersection? closestIntersection =
				scene.GetIntersections(ray, eRayMask.Visible)
				     .OrderBy(kvp => kvp.Value.Distance)
					 .Select(kvp => (Intersection?)kvp.Value)
				     .FirstOrDefault();

			if (closestIntersection == null)
				return Color.Black;

			Vector3 position = Vector3.Clamp(closestIntersection.Value.Position, Min, Max) - Min;
			Vector3 range = Max - Min;

			return Color.FromArgb((int)((position.X / range.X) * 255),
			                      (int)((position.Y / range.Y) * 255),
			                      (int)((position.Z / range.Z) * 255));
		}
	}
}
