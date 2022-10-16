using System.Collections.Generic;
using System.Linq;
using Raytracer.Layers;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Cameras;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Lights;

namespace Raytracer
{
	public sealed class Scene
	{
		public ICamera Camera { get; set; }
		public List<ILight> Lights { get; set; }
		public List<ISceneGeometry> Geometry { get; set; }
		public List<ILayer> Layers { get; set; }

		public int MaxReflectionRays { get; set; } = 10;
        public int Samples { get; set; } = 32;
        public int GlobalIlluminationSamples { get; set; } = 4;
		public int AmbientOcclusionSamples { get; set; } = 4;
		public float AmbientOcclusionScale { get; set; } = 1;

		public IEnumerable<KeyValuePair<ISceneGeometry, Intersection>> GetIntersections(
			Ray ray, eRayMask mask = eRayMask.All)
		{
			return Geometry.Where(g => (g.RayMask & mask) != eRayMask.None)
			               .SelectMany(g => g.GetIntersections(ray)
			                                 .Select(i => new KeyValuePair<ISceneGeometry, Intersection>(g, i)));
		}
	}
}
