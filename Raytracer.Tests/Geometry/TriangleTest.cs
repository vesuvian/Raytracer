using System.Linq;
using System.Numerics;
using NUnit.Framework;
using Raytracer.Geometry;

namespace Raytracer.Tests.Geometry
{
    [TestFixture]
    public class TriangleTest
    {
        private static readonly object[] s_ClipTestCases =
        {
            new object[]
            {
                new Triangle
                {
                    A = new Vertex
                    {
                        Position = new Vector3(0, 0, 0)
                    },
                    B = new Vertex
                    {
                        Position = new Vector3(0, 1, 0)
                    },
                    C = new Vertex
                    {
                        Position = new Vector3(1, 1, 0)
                    }
                },
                new Aabb
                {
                    Min = Vector3.Zero,
                    Max = Vector3.One
                },
                new Triangle[]
                {
                    new Triangle
                    {
                        A = new Vertex
                        {
                            Position = new Vector3(0, 0, 0)
                        },
                        B = new Vertex
                        {
                            Position = new Vector3(0, 1, 0)
                        },
                        C = new Vertex
                        {
                            Position = new Vector3(1, 1, 0)
                        }
                    }
                }
            },
            new object[]
            {
                new Triangle
                {
                    A = new Vertex
                    {
                        Position = new Vector3(0, 0, 0)
                    },
                    B = new Vertex
                    {
                        Position = new Vector3(0, 1, 0)
                    },
                    C = new Vertex
                    {
                        Position = new Vector3(1, 1, 0)
                    }
                },
                new Aabb
                {
                    Min = Vector3.Zero,
                    Max = Vector3.One / 2
                },
                new Triangle[]
                {
                    new Triangle
                    {
                        A = new Vertex
                        {
                            Position = new Vector3(0, 0, 0)
                        },
                        B = new Vertex
                        {
                            Position = new Vector3(0, 1, 0)
                        },
                        C = new Vertex
                        {
                            Position = new Vector3(1, 1, 0)
                        }
                    }
                }
            },
            new object[]
            {
                new Triangle
                {
                    A = new Vertex
                    {
                        Position = new Vector3(0, 0, 0)
                    },
                    B = new Vertex
                    {
                        Position = new Vector3(0, 1, 0)
                    },
                    C = new Vertex
                    {
                        Position = new Vector3(1, 1, 0)
                    }
                },
                new Aabb
                {
                    Min = Vector3.One * 10,
                    Max = Vector3.One * 10 + Vector3.One
                },
                new Triangle[]
                {
                }
            }
        };

        [TestCaseSource(nameof(s_ClipTestCases))]
        public void ClipTest(Triangle triangle, Aabb aabb, Triangle[] expected)
        {
            Triangle[] clipped = triangle.Clip(aabb).ToArray();
            Assert.AreEqual(expected, clipped);
        }
    }
}
