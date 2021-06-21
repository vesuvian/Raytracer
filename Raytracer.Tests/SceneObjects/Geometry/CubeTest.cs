using System.Numerics;
using NUnit.Framework;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Tests.SceneObjects.Geometry
{
    [TestFixture]
    public sealed class CubeTest
    {
	    private static readonly object[] s_GetIntersectionTestCases =
	    {
		    new object[]
		    {
			    new Cube(),
			    new Ray
			    {
				    Origin = new Vector3(0, -1, 0),
				    Direction = new Vector3(0, 1, 0)
			    },
			    true,
			    new Intersection
			    {
				    Normal = new Vector3(0, -1, 0),
				    Position = new Vector3(0, -0.5f, 0),
				    RayOrigin = new Vector3(0, -1, 0)
			    }
		    },
		    new object[]
		    {
			    new Cube(),
			    new Ray
			    {
				    Origin = new Vector3(0, 0, -10),
				    Direction = new Vector3(0, 0, 1)
			    },
			    true,
			    new Intersection
			    {
				    Normal = new Vector3(0, 0, -1),
				    Position = new Vector3(0, 0, -0.5f),
				    RayOrigin = new Vector3(0, 0, -10)
			    }
		    },
		    new object[]
		    {
			    new Cube
			    {
					Scale = new Vector3(2, 2, 2)
			    },
			    new Ray
			    {
				    Origin = new Vector3(0, 0, -10),
				    Direction = new Vector3(0, 0, 1)
			    },
			    true,
			    new Intersection
			    {
				    Normal = new Vector3(0, 0, -1),
				    Position = new Vector3(0, 0, -1f),
				    RayOrigin = new Vector3(0, 0, -10)
			    }
		    }
		};

		[TestCaseSource(nameof(s_GetIntersectionTestCases))]
	    public static void GetIntersection(Cube cube, Ray ray, bool expected, Intersection expectedIntersection)
	    {
		    Intersection intersection;
		    bool result = cube.GetIntersection(ray, out intersection);

			Assert.AreEqual(expected, result);
			Assert.AreEqual(expectedIntersection, intersection);
	    }
    }
}
