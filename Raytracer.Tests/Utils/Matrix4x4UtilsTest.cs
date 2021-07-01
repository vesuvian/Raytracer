using System.Numerics;
using NUnit.Framework;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Tests.Utils
{
	[TestFixture]
	public sealed class Matrix4x4UtilsTest
	{
		private static readonly object[] s_TbnTestCases =
		{
			new object[]
			{
				new Vector3(1, 0, 0),
				new Vector3(0, 0, 1),
				new Vector3(0, 1, 0),
				new Vector3(0, 1, 0),
				new Vector3(0, 1, 0)
			},
			new object[]
			{
				new Vector3(1, 0, 0),
				new Vector3(0, 0, 1),
				new Vector3(0, 1, 0),
				new Vector3(0, 0, -1),
				new Vector3(0, 0, -1)
			},
		};

		[TestCaseSource(nameof(s_TbnTestCases))]
		public static void Tbn(Vector3 tangent, Vector3 bitangent, Vector3 normal, Vector3 surfaceNormal, Vector3 expected)
		{
			Matrix4x4 surfaceMatrix = Matrix4x4Utils.Tbn(tangent, bitangent, normal);
			Vector3 result = surfaceMatrix.MultiplyNormal(surfaceNormal);
			Assert.AreEqual(expected, result);
		}
	}
}
