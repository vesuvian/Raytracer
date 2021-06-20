using System.Collections.Generic;
using Raytracer.Layers;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer
{
	public class Scene
	{
		public Camera Camera { get; set; }
		public List<Light> Lights { get; set; }
		public List<ISceneGeometry> Geometry { get; set; }
		public List<ILayer> Layers { get; set; }
	}
}
