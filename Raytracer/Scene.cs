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
		public int GlobalIlluminationSamples { get; set; } = 4;
		public int AmbientOcclusionSamples { get; set; } = 4;
		public float AmbientOcclusionScale { get; set; } = 1;

		private BoundingVolumeHierarchy m_BoundingVolumeHierarchy;

        public void Initialize()
        {
            m_BoundingVolumeHierarchy = new BoundingVolumeHierarchy(Geometry);
        }

		public IEnumerable<Intersection> GetIntersections(Ray ray, eRayMask mask = eRayMask.All, float minDelta = float.NegativeInfinity,
                                                          float maxDelta = float.PositiveInfinity, bool ordered = true)
        {
            IEnumerable<Intersection> intersections = m_BoundingVolumeHierarchy.GetIntersections(ray, mask, minDelta, maxDelta);

            if (ordered)
                intersections = intersections.OrderBy(i => i.RayDelta);

            return intersections;
        }
	}
}
