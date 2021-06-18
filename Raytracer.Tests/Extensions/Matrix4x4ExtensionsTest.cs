using System.Numerics;
using NUnit.Framework;
using Raytracer.Extensions;
using Raytracer.Utils;

namespace Raytracer.Tests.Extensions
{
	[TestFixture]
// ReSharper disable InconsistentNaming
	public sealed class Matrix4x4ExtensionsTest
// ReSharper disable InconsistentNaming
	{
		private static readonly object[] s_MultiplyNormalTestCases =
		{
		};

		private static readonly object[] s_MultiplyDirectionTestCases =
		{
		};

		private static readonly object[] s_MultiplyPointTestCases =
		{
			new object[] 
			{
				Matrix4x4.CreateTranslation(1, 0, 0),
				Vector3.Zero,
				new Vector3(1, 0, 0)
			},
			new object[]
			{
				Matrix4x4.CreateTranslation(0, 1, 0),
				Vector3.Zero,
				new Vector3(0, 1, 0)
			},
			new object[]
			{
				Matrix4x4.CreateTranslation(0, 0, 1),
				Vector3.Zero,
				new Vector3(0, 0, 1)
			},
			new object[]
			{
				Matrix4x4.CreateScale(2, 1, 1),
				Vector3.One,
				new Vector3(2, 1, 1)
			},
			new object[]
			{
				Matrix4x4.CreateScale(1, 2, 1),
				Vector3.One,
				new Vector3(1, 2, 1)
			},
			new object[]
			{
				Matrix4x4.CreateScale(1, 1, 2),
				Vector3.One,
				new Vector3(1, 1, 2)
			},
			new object[]
			{
				Matrix4x4.CreateFromYawPitchRoll(MathUtils.DEG2RAD * 90, 0, 0),
				new Vector3(0, 0, 1),
				new Vector3(1, 0, 0)
			},
			new object[]
			{
				Matrix4x4.CreateFromYawPitchRoll(0, MathUtils.DEG2RAD * 90, 0),
				new Vector3(0, 0, 1),
				new Vector3(0, -1, 0)
			},
			new object[]
			{
				Matrix4x4.CreateFromYawPitchRoll(0, 0, MathUtils.DEG2RAD * 90),
				new Vector3(0, 1, 0),
				new Vector3(-1, 0, 0)
			},
		};

		[TestCaseSource(nameof(s_MultiplyNormalTestCases))]
		public static void MultiplyNormal(Matrix4x4 matrix, Vector3 vector, Vector3 expected)
		{
			Vector3 result = matrix.MultiplyNormal(vector);

			Assert.AreEqual(expected.X, result.X, 0.0001);
			Assert.AreEqual(expected.Y, result.Y, 0.0001);
			Assert.AreEqual(expected.Z, result.Z, 0.0001);
		}

		[TestCaseSource(nameof(s_MultiplyDirectionTestCases))]
		public static void MultiplyDirection(Matrix4x4 matrix, Vector3 vector, Vector3 expected)
		{
			Vector3 result = matrix.MultiplyDirection(vector);

			Assert.AreEqual(expected.X, result.X, 0.0001);
			Assert.AreEqual(expected.Y, result.Y, 0.0001);
			Assert.AreEqual(expected.Z, result.Z, 0.0001);
		}

		[TestCaseSource(nameof(s_MultiplyPointTestCases))]
		public static void MultiplyPoint(Matrix4x4 matrix, Vector3 point, Vector3 expected)
		{
			Vector3 result = matrix.MultiplyPoint(point);

			Assert.AreEqual(expected.X, result.X, 0.0001);
			Assert.AreEqual(expected.Y, result.Y, 0.0001);
			Assert.AreEqual(expected.Z, result.Z, 0.0001);
		}
	}
}
