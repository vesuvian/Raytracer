using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using Raytracer.Math;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Tests.SceneObjects.Geometry
{
	[TestFixture]
	public sealed class SphereTest
	{
		private static readonly object[] s_GetIntersectionTestCases =
		{
			new object[]
			{
				new Sphere(),
				new Ray
				{
					Origin = new Vector3(0, 0, -10),
					Direction = new Vector3(0, 0, 1)
				},
				new[]
				{
					new Intersection
					{
						Normal = new Vector3(0, 0, -1),
						Position = new Vector3(0, 0, -1),
						Ray = new Ray { Origin = new Vector3(0, 0, -10), Direction = new Vector3(0, 0, 1)}
					}
				}
			},
			new object[]
			{
				new Sphere
				{
					Scale = new Vector3(2, 2, 2)
				},
				new Ray
				{
					Origin = new Vector3(0, 0, -10),
					Direction = new Vector3(0, 0, 1)
				},
				new[]
				{
					new Intersection
					{
						Normal = new Vector3(0, 0, -1),
						Position = new Vector3(0, 0, -2f),
						Ray = new Ray { Origin = new Vector3(0, 0, -10), Direction = new Vector3(0, 0, 1)}
					}
				}
			}
		};

		[TestCaseSource(nameof(s_GetIntersectionTestCases))]
		public static void GetIntersection(Sphere sphere, Ray ray, IEnumerable<Intersection> expectedIntersections)
		{
			IEnumerable<Intersection> intersections = sphere.GetIntersections(ray);
			CollectionAssert.AreEqual(expectedIntersections, intersections);
		}
	}
}