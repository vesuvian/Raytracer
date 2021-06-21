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
				true,
				new Intersection
				{
					Normal = new Vector3(0, 0, -1),
					Position = new Vector3(0, 0, -1),
					RayOrigin = new Vector3(0, 0, -10)
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
				true,
				new Intersection
				{
					Normal = new Vector3(0, 0, -1),
					Position = new Vector3(0, 0, -2f),
					RayOrigin = new Vector3(0, 0, -10)
				}
			}
		};

		[TestCaseSource(nameof(s_GetIntersectionTestCases))]
		public static void GetIntersection(Sphere sphere, Ray ray, bool expected, Intersection expectedIntersection)
		{
			Intersection intersection;
			bool result = sphere.GetIntersection(ray, out intersection);

			Assert.AreEqual(expected, result);
			Assert.AreEqual(expectedIntersection, intersection);
		}
	}
}