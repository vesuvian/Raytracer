using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Materials;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Geometry
{
	public sealed class Mesh
	{
		public List<Triangle> Triangles { get; set; } = new List<Triangle>();

        public IEnumerable<Intersection> GetIntersections(Ray ray, ISceneGeometry geometry, IMaterial material)
		{
			foreach (Triangle triangle in Triangles)
            {
                Intersection intersection;
                if (triangle.GetIntersection(ray, geometry, material, out intersection))
                    yield return intersection;
            }
		}

		public float CalculateSurfaceArea()
        {
            return Triangles.Aggregate(0.0f, (area, triangle) => area + triangle.SurfaceArea);
        }

        public Aabb CalculateAabb(Matrix4x4 localToWorld)
        {
            IEnumerable<Vector3> points = Triangles.SelectMany(t => new[] { t.A.Position, t.B.Position, t.C.Position });
            return Aabb.FromPoints(localToWorld, points);
        }

        public Mesh Clip(Matrix4x4 localToWorld, Aabb aabb)
        {
            return new Mesh
            {
                Triangles = Triangles.SelectMany(t => t.Multiply(localToWorld).Clip(aabb)).ToList()
            };
        }
    }
}
