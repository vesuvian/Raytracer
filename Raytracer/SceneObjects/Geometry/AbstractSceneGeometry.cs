using System.Collections.Generic;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Materials;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Geometry
{
	public abstract class AbstractSceneGeometry : AbstractSceneObject, ISceneGeometry
	{
		public IMaterial Material { get; set; } = new Material();

		public eRayMask RayMask { get; set; } = eRayMask.All;

		public abstract IEnumerable<Intersection> GetIntersections(Ray ray);

		public Vector3 GetSurfaceNormal(Intersection intersection)
		{
			// Hack - flip Z and Y since normal map uses Z for "towards"
			Vector3 normalMap = Material.SampleNormal(intersection.Uv);
			normalMap = Vector3.Normalize(new Vector3(normalMap.X * Material.NormalScale,
			                                          normalMap.Z * -1,
			                                          normalMap.Y * Material.NormalScale));

			// Get the normal in world space
			Matrix4x4 surface = Matrix4x4Utils.Tbn(intersection.Tangent, intersection.Bitangent, intersection.Normal);
			return surface.MultiplyNormal(normalMap);
		}
	}
}
