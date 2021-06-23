﻿using System.Collections.Generic;
using System.Linq;
using Raytracer.Layers;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry;
using Raytracer.SceneObjects.Lights;

namespace Raytracer
{
	public sealed class Scene
	{
		public Camera Camera { get; set; }
		public List<ILight> Lights { get; set; }
		public List<ISceneGeometry> Geometry { get; set; }
		public List<ILayer> Layers { get; set; }

		public IEnumerable<KeyValuePair<ISceneGeometry, Intersection>> GetIntersections(Ray ray)
		{
			return Geometry.SelectMany(g => g.GetIntersections(ray)
			                                 .Select(i => new KeyValuePair<ISceneGeometry, Intersection>(g, i)))
			               .OrderBy(kvp => kvp.Value.Distance);
		}
	}
}
