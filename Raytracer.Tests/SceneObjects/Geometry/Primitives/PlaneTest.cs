using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Raytracer.Math;
using Raytracer.SceneObjects;
using Raytracer.Utils;
using Raytracer.SceneObjects.Geometry.Primitives;

namespace Raytracer.Tests.SceneObjects.Geometry.Primitives
{
	[TestFixture]
	public sealed class PlaneTest
	{
		private static readonly object[] s_GetIntersectionTestCases =
		{
			new object[]
			{
				new PlaneSceneGeometry(),
				new Ray
				{
					Origin = new Vector3(0, 1, 0),
					Direction = new Vector3(0, -1, 0)
				},
				new[]
				{
					new Intersection
					{
						Normal = new Vector3(0, 1, 0),
						Position = Vector3.Zero,
						Ray = new Ray { Origin = new Vector3(0, 1, 0), Direction = new Vector3(0, 1, 0) }
					}
				}
			},
			new object[]
			{
				new PlaneSceneGeometry
				{
					Position = new Vector3(0, -1, 0)
				},
				new Ray
				{
					Origin = new Vector3(0, 0, 0),
					Direction = new Vector3(0, -1, 0)
				},
				new[]
				{
					new Intersection
					{
						Normal = new Vector3(0, 1, 0),
						Position = new Vector3(0, -1, 0),
						Ray = new Ray { Origin = new Vector3(0, 0, 0), Direction = new Vector3(0, -1, 0) }
					}
				}
			},
			new object[]
			{
				new PlaneSceneGeometry(),
				new Ray
				{
					Origin = new Vector3(0, -1, 0),
					Direction = new Vector3(0, 1, 0)
				},
				new[]
				{
					new Intersection
					{
						Normal = new Vector3(0, -1, 0),
						Position = Vector3.Zero,
						Ray = new Ray { Origin = new Vector3(0, -1, 0), Direction = new Vector3(0, 1, 0) }
					}
				}
			},
			new object[]
			{
				new PlaneSceneGeometry
				{
					Rotation = Quaternion.CreateFromYawPitchRoll(0, MathUtils.DEG2RAD * 45, 0)
				},
				new Ray
				{
					Origin = new Vector3(0, 1, -10),
					Direction = new Vector3(0, 0, 1)
				},
				new[]
				{
					new Intersection
					{
						Normal = new Vector3(0, -0.7071067f, -0.7071068f),
						Position = new Vector3(0, 0.99999946f, -1),
						Ray = new Ray { Origin = new Vector3(0, 1, -10), Direction = new Vector3(0, 0, 1) }
					}
				}
			}
		};

		[TestCaseSource(nameof(s_GetIntersectionTestCases))]
		public static void GetIntersection(PlaneSceneGeometry plane, Ray ray, IEnumerable<Intersection> expectedIntersections)
		{
			IEnumerable<Intersection> intersections = plane.GetIntersections(ray, eRayMask.All);
			CollectionAssert.AreEqual(expectedIntersections, intersections);
		}
	}
}
