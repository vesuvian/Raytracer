using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Materials;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Geometry
{
    [DebuggerDisplay("A = {A}, B = {B}, C = {C}")]
    public record Triangle
    {
        public Vertex A { get; set; } = new Vertex();
        public Vertex B { get; set; } = new Vertex();
        public Vertex C { get; set; } = new Vertex();

        public IEnumerable<Vertex> Vertices { get { return new [] { A, B, C }; } }
        public float SurfaceArea { get { return GetSurfaceArea(A.Position, B.Position, C.Position); } }

        public bool GetIntersection(Ray ray, ISceneGeometry geometry, IMaterial material, out Intersection intersection)
        {
            intersection = default;

            float t, u, v;
            if (!HitTriangle(A.Position, B.Position, C.Position, ray, out t, out u, out v))
                return false;

            Vertex interpolated = GetInterpolated(A, B, C, u, v);

            intersection = new Intersection
            {
                Position = interpolated.Position,
                Normal = interpolated.Normal,
                Tangent = interpolated.Tangent,
                Bitangent = interpolated.Bitangent,
                Uv = interpolated.Uv,
                Ray = ray,
                Geometry = geometry,
                Material = material
            };

            return true;
        }

        public Triangle Multiply(Matrix4x4 matrix)
        {
            return new Triangle
            {
                A = A.Multiply(matrix),
                B = B.Multiply(matrix),
                C = C.Multiply(matrix)
            };
        }

        public IEnumerable<Triangle> Clip(Aabb aabb)
        {
            return SutherlandHodgmanPolygonClip(this, aabb);
        }

        public static bool HitTriangle(Vector3 a, Vector3 b, Vector3 c, Ray ray, out float t, out float u, out float v)
        {
            t = default;
            u = default;
            v = default;

            // Get the plane normal
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 pvec = Vector3.Cross(ray.Direction, ac);

            // Ray and triangle are parallel if det is close to 0
            float det = Vector3.Dot(ab, pvec);
            if (MathF.Abs(det) < 0.00001f)
                return false;

            float invDet = 1 / det;

            Vector3 tvec = ray.Origin - a;
            u = Vector3.Dot(tvec, pvec) * invDet;
            if (u < 0 || u > 1)
                return false;

            Vector3 qvec = Vector3.Cross(tvec, ab);
            v = Vector3.Dot(ray.Direction, qvec) * invDet;
            if (v < 0 || u + v > 1)
                return false;

            t = Vector3.Dot(ac, qvec) * invDet;
            return true;
        }

		public static Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Normalize(Vector3.Cross(b - a, c - a));
        }

        public static Vector3 GetInterpolated(Vector3 a, Vector3 b, Vector3 c, float u, float v)
        {
            return (1 - u - v) * a + u * b + v * c;
        }

        public static Vector2 GetInterpolated(Vector2 a, Vector2 b, Vector2 c, float u, float v)
        {
            return (1 - u - v) * a + u * b + v * c;
        }

        public static Vertex GetInterpolated(Vertex a, Vertex b, Vertex c, float u, float v)
        {
            return new Vertex
            {
                Position = GetInterpolated(a.Position, b.Position, c.Position, u, v),
                Normal = Vector3.Normalize(GetInterpolated(a.Normal, b.Normal, c.Normal, u, v)),
                Tangent = Vector3.Normalize(GetInterpolated(a.Tangent, b.Tangent, c.Tangent, u, v)),
                Uv = GetInterpolated(a.Uv, b.Uv, c.Uv, u, v)
            };
        }

        private static Vertex GetInterpolated(Vertex a, Vertex b, float t)
        {
            return new Vertex
            {
                Position = Vector3.Lerp(a.Position, b.Position, t),
                Normal = Vector3.Normalize(Vector3.Lerp(a.Normal, b.Normal, t)),
                Tangent = Vector3.Normalize(Vector3.Lerp(a.Tangent, b.Tangent, t)),
                Uv = Vector2.Lerp(a.Uv, b.Uv, t)
            };
        }

        public static float GetSurfaceArea(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 bc = c - b;

            float lengthA = ab.Length();
            float lengthB = ac.Length();
            float lengthC = bc.Length();

            float abcOver2 = (lengthA + lengthB + lengthC) / 2;

            return MathF.Sqrt(abcOver2 * (abcOver2 - lengthA) * (abcOver2 - lengthB) * (abcOver2 - lengthC));
        }

        /// <summary>
        /// Clips the given triangle against the given clipping planes.
        /// Returns a fanned triangle mesh for the resulting polygon.
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="aabb"></param>
        /// <returns></returns>
        private static IEnumerable<Triangle> SutherlandHodgmanPolygonClip(Triangle triangle, Aabb aabb)
        {
            IEnumerable<Vertex> verts = SutherlandHodgmanPolygonClip(triangle.Vertices, aabb);
            return BuildConvexTriangleFan(verts);
        }

        /// <summary>
        /// Clips the given polygon against the given clipping planes.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="aabb"></param>
        /// <returns></returns>
        private static IEnumerable<Vertex> SutherlandHodgmanPolygonClip(IEnumerable<Vertex> polygon, Aabb aabb)
        {
            List<Vertex> clipped = polygon.ToList();

            var planes = aabb.Planes.ToArray();

            foreach (Plane clippingPlane in aabb.Planes)
            {
                if (clipped.Count == 0)
                    break;

                List<Vertex> input = clipped;
                clipped = new List<Vertex>();

                for (int index = 0; index < input.Count; index++)
                {
                    // Loop the vertices in pairs
                    Vertex a = input[index];
                    Vertex b = input[(index + 1) % input.Count];

                    bool aInFront = !clippingPlane.IsBehind(a.Position);
                    bool bInFront = !clippingPlane.IsBehind(b.Position);

                    // Line isn't clipped, add the second vertex
                    if (aInFront && bInFront)
                    {
                        clipped.Add(b);
                        continue;
                    }

                    // Find the point where the line intersects the plane
                    Ray ray = new Ray
                    {
                        Origin = a.Position,
                        Direction = Vector3.Normalize(b.Position - a.Position)
                    };

                    // Line is parallel to the plane, add the second vertex
                    float t;
                    if (!clippingPlane.GetIntersection(ray, out t))
                    {
                        clipped.Add(b);
                        continue;
                    }

                    // Find the interpolated vertex at the intersection
                    Vertex interpolated = GetInterpolated(a, b, t);

                    // A is inside
                    if (aInFront)
                    {
                        if (interpolated != a)
                            clipped.Add(interpolated);
                        continue;
                    }

                    // B is inside
                    if (bInFront)
                    {
                        if (interpolated != b)
                            clipped.Add(interpolated);

                        clipped.Add(b);
                    }
                }
            }

            return clipped;
        }

        /// <summary>
        /// Given a convex shape defined by the given sequence of vertices, returns a contiguous triangle fan. 
        /// </summary>
        /// <param name="verts"></param>
        /// <returns></returns>
        private static IEnumerable<Triangle> BuildConvexTriangleFan(IEnumerable<Vertex> verts)
        {
            Vertex a = default;
            Vertex b = default;
            Vertex c = default;

            int index = 0;

            foreach (Vertex vert in verts)
            {
                if (index == 0)
                    a = vert;

                if (index % 2 == 0)
                    c = vert;
                else
                    b = vert;

                if (index >= 2)
                    yield return new Triangle { A = a, B = b, C = c };

                index++;
            }
        }
    }
}
