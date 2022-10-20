using System.Numerics;
using NUnit.Framework;
using Raytracer.Extensions;
using Raytracer.Math;

namespace Raytracer.Tests.Extensions
{
    [TestFixture]
    public class PlaneExtensionsTest
    {
        private static readonly object[] s_IsInFrontCases =
        {
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, 1, 0),
                true
            },
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, 0, 0),
                false
            },
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, -1, 0),
                false
            }
        };

        private static readonly object[] s_IsBehindCases =
        {
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, 1, 0),
                false
            },
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, 0, 0),
                false
            },
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, -1, 0),
                true
            }
        };

        private static readonly object[] s_DistanceCases =
        {
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, 1, 0),
                1
            },
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, 0, 0),
                0
            },
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Vector3(0, -1, 0),
                1
            },
            new object[]
            {
	            new Plane(Vector3.UnitY, 1),
	            new Vector3(0, 1, 0),
	            0
            },
            new object[]
            {
	            new Plane(-Vector3.UnitY, 1),
	            new Vector3(0, -1, 0),
	            0
            },
        };

        private static readonly object[] s_GetIntersectionCases =
        {
            new object[]
            {
                new Plane(Vector3.UnitY, 0),
                new Ray(-Vector3.UnitY, Vector3.UnitY),
                true,
                1
            },
        };

        [TestCaseSource(nameof(s_IsInFrontCases))]
        public void IsInFront(Plane plane, Vector3 point, bool expected)
        {
            bool result = plane.IsInFront(point);
            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(nameof(s_IsBehindCases))]
        public void IsBehind(Plane plane, Vector3 point, bool expected)
        {
            bool result = plane.IsBehind(point);
            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(nameof(s_DistanceCases))]
        public void Distance(Plane plane, Vector3 point, float expected)
        {
            float result = plane.Distance(point);
            Assert.AreEqual(expected, result);
        }

        [TestCaseSource(nameof(s_GetIntersectionCases))]
        public void GetIntersection(Plane plane, Ray ray, bool expected, float expectedT)
        {
            float t;
            bool result = plane.GetIntersection(ray, out t);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(expectedT, t);
        }
    }
}
