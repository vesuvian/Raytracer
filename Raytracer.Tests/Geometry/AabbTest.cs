using System.Numerics;
using NUnit.Framework;
using Raytracer.Extensions;
using Raytracer.Geometry;

namespace Raytracer.Tests.Geometry
{
	[TestFixture]
	public class AabbTest
	{
        private static readonly object[] s_ClipLineTestCases =
       {
            new object[]
            {
                new Aabb
                (
					new Vector3(0, 0, 0),
					new Vector3(1, 1, 1)
                ),
				new Vector3(-1, 0.5f, 0.5f),
				new Vector3(2, 0.5f, 0.5f),
				true,
				new Vector3(0, 0.5f, 0.5f),
				new Vector3(1, 0.5f, 0.5f)
			},
            new object[]
            {
	            new Aabb
	            (
		            new Vector3(0, 0, 0),
		            new Vector3(1, 1, 1)
	            ),
	            new Vector3(-1, 2f, 0.5f),
	            new Vector3(2, 2f, 0.5f),
	            false,
	            new Vector3(0),
	            new Vector3(0)
            },
            new object[]
            {
	            new Aabb
	            (
		            new Vector3(0, 0, 0),
		            new Vector3(1, 1, 1)
	            ),
	            new Vector3(-3, 0.5f, 0.5f),
	            new Vector3(-1, 0.5f, 0.5f),
	            false,
	            new Vector3(0),
	            new Vector3(0)
			},
            new object[]
            {
	            new Aabb
	            (
		            new Vector3(0, 0, 0),
		            new Vector3(1, 1, 1)
	            ),
	            new Vector3(2, 0.5f, 0.5f),
	            new Vector3(3, 0.5f, 0.5f),
	            false,
	            new Vector3(0),
	            new Vector3(0)
            },
            new object[]
            {
	            new Aabb
	            (
		            new Vector3(0, 0, 0),
		            new Vector3(1, 1, 1)
	            ),
	            new Vector3(-0.5f, 0.5f, 0.5f),
	            new Vector3(0.5f, 0.5f, 0.5f),
	            true,
	            new Vector3(0, 0.5f, 0.5f),
	            new Vector3(0.5f, 0.5f, 0.5f)
            },
		};

		[Test]
        public void PlanesTest()
        {
	        Aabb aabb = new Aabb(Vector3.Zero, Vector3.One);
			Vector3 point = Vector3.One / 2;

			foreach (Plane plane in aabb.Planes)
				Assert.True(plane.IsInFront(point));
        }

        [TestCaseSource(nameof(s_ClipLineTestCases))]
		public void ClipLineTest(Aabb aabb, Vector3 a, Vector3 b, bool expected, Vector3 expectedA, Vector3 expectedB)
		{
			Vector3 clippedA;
			Vector3 clippedB;
			bool result = aabb.ClipLine(a, b, out clippedA, out clippedB);

			Assert.AreEqual(expected, result);
			Assert.AreEqual(expectedA, clippedA);
			Assert.AreEqual(expectedB, clippedB);
		}
	}
}
