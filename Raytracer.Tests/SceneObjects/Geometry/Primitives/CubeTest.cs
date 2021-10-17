using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.SceneObjects.Geometry.Primitives;
using Raytracer.Utils;

namespace Raytracer.Tests.SceneObjects.Geometry.Primitives
{
    [TestFixture]
    public sealed class CubeTest
    {
	    private static readonly object[] s_GetIntersectionTestCases =
	    {
		    new object[]
		    {
			    new CubeSceneGeometry(),
			    new Ray
			    {
				    Origin = new Vector3(0, -1, 0),
				    Direction = new Vector3(0, 1, 0)
			    },
				new []
				{
				    new Intersection
				    {
					    Normal = new Vector3(0, -1, 0),
					    Position = new Vector3(0, -0.5f, 0),
						Ray = new Ray { Origin = new Vector3(0, -1, 0), Direction = new Vector3(0, 1, 0)}
					}
				}
		    },
		    new object[]
		    {
			    new CubeSceneGeometry(),
			    new Ray
			    {
				    Origin = new Vector3(0, 0, -10),
				    Direction = new Vector3(0, 0, 1)
			    },
			    new []
			    {
					new Intersection
				    {
					    Normal = new Vector3(0, 0, -1),
					    Position = new Vector3(0, 0, -0.5f),
					    Ray = new Ray { Origin = new Vector3(0, 0, -10), Direction = new Vector3(0, 0, 1)}
				    }
				}
		    },
		    new object[]
		    {
			    new CubeSceneGeometry
				{
					Scale = new Vector3(2, 2, 2)
			    },
			    new Ray
			    {
				    Origin = new Vector3(0, 0, -10),
				    Direction = new Vector3(0, 0, 1)
			    },
			    new []
			    {
					new Intersection
				    {
					    Normal = new Vector3(0, 0, -1),
					    Position = new Vector3(0, 0, -1f),
						Ray = new Ray { Origin = new Vector3(0, 0, -10), Direction = new Vector3(0, 0, 1)}
					}
				}
		    },
		    new object[]
		    {
			    new CubeSceneGeometry
				{
					Rotation = Quaternion.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 90, MathUtils.DEG2RAD * 90, MathUtils.DEG2RAD * 90)
			    },
			    new Ray
			    {
				    Origin = new Vector3(0, 0, -10),
				    Direction = new Vector3(0, 0, 1)
			    },
			    new []
			    {
					new Intersection
				    {
					    Normal = new Vector3(0, 0, -1),
					    Position = new Vector3(0, 0, -0.5f),
						Ray = new Ray { Origin = new Vector3(0, 0, -10), Direction = new Vector3(0, 0, 1)}
					}
				}
		    },
		};

		[TestCaseSource(nameof(s_GetIntersectionTestCases))]
	    public static void GetIntersections(CubeSceneGeometry cube, Ray ray, IEnumerable<Intersection> expectedIntersections)
	    {
		    IEnumerable<Intersection> intersections = cube.GetIntersections(ray, eRayMask.All);
		    CollectionAssert.AreEqual(expectedIntersections, intersections);
	    }
    }
}
