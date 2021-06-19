using System.Numerics;
using NUnit.Framework;
using Raytracer.Math;
using Plane = Raytracer.SceneObjects.Geometry.Plane;

namespace Raytracer.Tests.SceneObjects.Geometry
{
    [TestFixture]
    public sealed class PlaneTest
    {
	    private static readonly object[] s_GetIntersectionTestCases =
	    {
		    new object[]
		    {
			    new Plane(),
			    new Ray
			    {
				    Origin = new Vector3(0, 1, 0),
				    Direction = new Vector3(0, -1, 0)
			    },
			    true,
			    new Intersection
			    {
				    Normal = new Vector3(0, 1, 0),
				    Position = Vector3.Zero,
				    RayOrigin = new Vector3(0, 1, 0)
			    }
		    },
			new object[]
			{
				new Plane
				{
					Position = new Vector3(0, -1, 0)
				},
				new Ray
				{
					Origin = new Vector3(0, 0, 0),
					Direction = new Vector3(0, -1, 0)
				},
				true,
				new Intersection
				{
					Normal = new Vector3(0, 1, 0),
					Position = new Vector3(0, -1, 0),
					RayOrigin = new Vector3(0, 0, 0)
				}
			},
			new object[]
			{
				new Plane(),
				new Ray
				{
					Origin = new Vector3(0, -1, 0),
					Direction = new Vector3(0, 1, 0)
				},
				true,
				new Intersection
				{
					Normal = new Vector3(0, -1, 0),
					Position = Vector3.Zero,
					RayOrigin = new Vector3(0, -1, 0)
				}
			},
		};


		[TestCaseSource(nameof(s_GetIntersectionTestCases))]
		public static void GetIntersection(Plane plane, Ray ray, bool expected, Intersection expectedIntersection)
	    {
		    Intersection intersection;
		    bool result = plane.GetIntersection(ray, out intersection);

			Assert.AreEqual(expected, result);
			Assert.AreEqual(expectedIntersection, intersection);
	    }
    }
}
