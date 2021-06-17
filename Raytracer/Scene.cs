using System.Collections.Generic;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer
{
	public class Scene
	{
		public Camera Camera { get; set; }
		public List<Light> Lights { get; set; }
		public List<IGeometry> Geometry { get; set; }
	}
}
