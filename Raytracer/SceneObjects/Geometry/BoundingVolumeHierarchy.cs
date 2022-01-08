using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Raytracer.Extensions;
using Raytracer.Geometry;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Geometry
{
	public sealed class BoundingVolumeHierarchy : ISceneGeometry
	{
		private readonly HashSet<ISceneGeometry> m_Leaves;

		private BoundingVolumeHierarchy m_Left;
		private BoundingVolumeHierarchy m_Right;

		private eRayMask? m_RayMask;
		private float? m_SurfaceArea;
		private Aabb? m_Aabb;

		#region Properties

		public IEnumerable<ISceneGeometry> Leaves { get { return m_Leaves.ToArray(); } }

		public BoundingVolumeHierarchy Left { get { return m_Left; } }

		public BoundingVolumeHierarchy Right { get { return m_Right; } }

		private IEnumerable<ISceneGeometry> Children
		{
			get
			{
				foreach (ISceneGeometry leaf in m_Leaves)
					yield return leaf;

				if (m_Left != null)
					yield return m_Left;

				if (m_Right != null)
					yield return m_Right;
			}
		}

		public eRayMask RayMask
		{
			get
			{
				m_RayMask ??= Children.Aggregate(eRayMask.None, (mask, child) => mask | child.RayMask);
				return m_RayMask.Value;
			}
		}

		public float SurfaceArea
		{
			get
			{
				m_SurfaceArea ??= Children.Aggregate(0.0f, (area, child) => area + child.SurfaceArea);
				return m_SurfaceArea.Value;
			}
		}

		public Aabb Aabb
		{
			get
			{
				if (!Children.Any())
					return default;
				return m_Aabb ??= Children.Select(c => c.Aabb).Sum();
			}
		}

		Vector3 ISceneObject.Position { get => Vector3.Zero; set => throw new NotSupportedException(); }

		Vector3 ISceneObject.Scale { get => Vector3.One; set => throw new NotSupportedException(); }

		Quaternion ISceneObject.Rotation { get => Quaternion.Identity; set => throw new NotSupportedException(); }

		Matrix4x4 ISceneObject.LocalToWorld => Matrix4x4.Identity;

		Matrix4x4 ISceneObject.WorldToLocal => Matrix4x4.Identity;

		Vector3 ISceneObject.Forward => Vector3.UnitZ;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		public BoundingVolumeHierarchy(IEnumerable<ISceneGeometry> geometry, int maxDepth = 10,
									   Action<IEnumerable<ISceneGeometry>, IEnumerable<ISceneGeometry>> onSlice = null)
		{
			m_Leaves = new HashSet<ISceneGeometry>();

			ProcessGeometry(geometry, maxDepth, onSlice);
		}

		#region Methods

		private class AabbRayIntersection
		{
			public ISceneGeometry Geometry { get; set; }
			public bool Hit { get; set; }
			public float MinDelta { get; set; }
			public float MaxDelta { get; set; }
		}

		public bool GetIntersection(Ray ray, eRayMask mask, out Intersection intersection, float minDelta = float.NegativeInfinity,
									float maxDelta = float.PositiveInfinity, bool testAabb = true)
		{
			intersection = default;

			if ((RayMask & mask) == eRayMask.None)
				return false;

			if (testAabb)
			{
				float tMin;
				float tMax;
				if (!Aabb.Intersects(ray, out tMin, out tMax))
					return false;

				if ((tMin < minDelta && tMax < minDelta) ||
					(tMin > maxDelta && tMax > maxDelta))
					return false;
			}

			// Get the children that intersect by distance
			IEnumerable<AabbRayIntersection> aabbRayIntersections =
				Children.Select(n =>
					 {
						 float tMin;
						 float tMax;
						 bool hit = Aabb.Intersects(ray, out tMin, out tMax);

						 return new AabbRayIntersection
						 {
							 MinDelta = tMin,
							 MaxDelta = tMax,
							 Hit = hit,
							 Geometry = n
						 };
					 })
					 .Where(i => i.Hit)
					 .OrderBy(i => MathF.Min(i.MinDelta, i.MaxDelta));

			// Now find the best intersection by actual geometry
			intersection = default;
			float bestT = float.MaxValue;
			bool found = false;

			foreach (var aabbRayIntersection in aabbRayIntersections)
			{
				if (aabbRayIntersection.MaxDelta < minDelta ||
				    aabbRayIntersection.MinDelta > maxDelta)
					continue;

				Intersection thisIntersection;
				if (!aabbRayIntersection.Geometry.GetIntersection(ray, mask, out thisIntersection, minDelta, maxDelta, false))
					continue;

				float delta = thisIntersection.RayDelta;
				maxDelta = MathF.Min(delta, maxDelta);

				if (delta < minDelta || delta > maxDelta)
					continue;

				if (delta > bestT)
					continue;

				bestT = thisIntersection.RayDelta;
				found = true;
				intersection = thisIntersection;
			}

			return found;
		}

		public IEnumerable<BoundingVolumeHierarchy> GetNodesRecursive()
		{
			IEnumerable<BoundingVolumeHierarchy> left = Left == null
				? Enumerable.Empty<BoundingVolumeHierarchy>()
				: Left.GetNodesRecursive().Prepend(Left);

			IEnumerable<BoundingVolumeHierarchy> right = Right == null
				? Enumerable.Empty<BoundingVolumeHierarchy>()
				: Right.GetNodesRecursive().Prepend(Right);

			return left.Concat(right);
		}

		#endregion

		#region Private Methods

		private void ProcessGeometry(IEnumerable<ISceneGeometry> geometry, int maxDepth, Action<IEnumerable<ISceneGeometry>, IEnumerable<ISceneGeometry>> onSlice)
		{
			HashSet<ISceneGeometry> toProcess = geometry.ToHashSet();
			onSlice ??= (l, r) => { };

			// Depth is zero
			if (maxDepth == 0)
			{
				m_Leaves.AddRange(toProcess);
				return;
			}

			// Find the best way to slice the remaining items in half
			eAxis bestAxis;
			float position = FindSlicePosition(toProcess, maxDepth, out bestAxis);

			// Perform the slice
			HashSet<ISceneGeometry> leaves;
			HashSet<ISceneGeometry> left;
			HashSet<ISceneGeometry> right;
			Slice(toProcess, position, bestAxis, out leaves, out left, out right);

			onSlice(left, right);

			m_Leaves.AddRange(leaves);

			Action[] recurse =
			{
				() =>
				{
					if (left.Count > 0)
						m_Left = new BoundingVolumeHierarchy(left, maxDepth - 1, onSlice);
				},
				() =>
				{
					if (right.Count > 0)
						m_Right = new BoundingVolumeHierarchy(right, maxDepth - 1, onSlice);
				}
			};

			Parallel.ForEach(recurse, r => r());
		}

		/// <summary>
		/// Finds the best way to bisect the given items in half.
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="maxDepth"></param>
		/// <param name="bestAxis"></param>
		/// <returns></returns>
		private static float FindSlicePosition(IEnumerable<ISceneGeometry> geometry, int maxDepth, out eAxis bestAxis)
		{
			IList<ISceneGeometry> items = geometry as IList<ISceneGeometry> ?? geometry.ToList();

			float bestPosition = 0;
			bestAxis = eAxis.X;

			float bestError = float.MaxValue;

			foreach (eAxis axis in Enum.GetValues(typeof(eAxis)))
			{
				float error;
				float position = FindSlicePosition(items, axis, out error);

				error += System.Math.Abs((maxDepth % 3) - (int)axis);

				if (error >= bestError)
					continue;
				
				bestAxis = axis;
				bestError = error;
				bestPosition = position;
			}

			return bestPosition;
		}

		/// <summary>
		/// Walk along the axis to find the best split.
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="axis"></param>
		/// <param name="error"></param>
		/// <returns></returns>
		private static float FindSlicePosition(IEnumerable<ISceneGeometry> geometry, eAxis axis, out float error)
		{
			ISceneGeometry[] sorted =
				geometry.Distinct()
						.Where(g => !g.Aabb.IsInfinite) // Don't consider infinite shapes for the slice position
						.OrderBy(g => g.Aabb.Min.GetValue(axis))
						.ToArray();

			const int resolution = 5;
			float[] edges =
				sorted.SelectMany(g =>
					  {
						  float min = g.Aabb.Min.GetValue(axis);
						  float max = g.Aabb.Max.GetValue(axis);
						  return Enumerable.Range(0, resolution)
										   .Select(i => MathUtils.Lerp(min, max, i / (float)(resolution - 1)));
					  })
					  .Distinct()
					  .OrderBy(e => e)
					  .ToArray();

			// Find the best edge to slice on
			error = float.MaxValue;
			float bestPosition = 0;

			for (int index = 1; index < edges.Length - 1; index++)
			{
				float position = edges[index];

				HashSet<ISceneGeometry> leaves;
				HashSet<ISceneGeometry> left;
				HashSet<ISceneGeometry> right;
				Slice(sorted, position, axis, out leaves, out left, out right);

				// Items that couldn't be split count towards the error
				float leafError = CalculateLeafError(leaves, sorted);

				// Surface area should be balanced between nodes
				float surfaceAreaError = CalculateSurfaceAreaError(left, right);

				// We should have a split
				float splitError = 0;
				if (left.Count == 0 || right.Count == 0)
					splitError = leaves.Count;

				float thisError = leafError + surfaceAreaError + splitError;
				//Trace.WriteLine($"{axis} - {position} - {thisError}");

				if (thisError >= error)
					continue;

				error = thisError;
				bestPosition = position;
			}

			return bestPosition;
		}

		private static float CalculateLeafError(IEnumerable<ISceneGeometry> leaves, IEnumerable<ISceneGeometry> total)
		{
			return leaves.Count() / (float)total.Count();
		}

		private static float CalculateSurfaceAreaError(IEnumerable<ISceneGeometry> left, IEnumerable<ISceneGeometry> right)
		{
			float leftSurfaceArea = left.Aggregate(0.0f, (area, g) => area + g.SurfaceArea);
			float rightSurfaceArea = right.Aggregate(0.0f, (area, g) => area + g.SurfaceArea);
			float totalSurfaceArea = leftSurfaceArea + rightSurfaceArea;
			return
				totalSurfaceArea == 0
					? 0
					: MathF.Abs(leftSurfaceArea - rightSurfaceArea) / (leftSurfaceArea + rightSurfaceArea);
		}

		/// <summary>
		/// Splits the given items along the given axis.
		/// </summary>
		/// <param name="geometry"></param>
		/// <param name="position"></param>
		/// <param name="axis"></param>
		/// <param name="leaves"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private static void Slice(IEnumerable<ISceneGeometry> geometry, float position, eAxis axis,
								  out HashSet<ISceneGeometry> leaves, out HashSet<ISceneGeometry> left,
								  out HashSet<ISceneGeometry> right)
		{
			leaves = new HashSet<ISceneGeometry>();
			left = new HashSet<ISceneGeometry>();
			right = new HashSet<ISceneGeometry>();

			foreach (ISceneGeometry item in geometry.Distinct())
			{
				Aabb aabb = item.Aabb;

				Aabb leftAabb = aabb;
				leftAabb.Max = leftAabb.Max.SetValue(axis, position);

				Aabb rightAabb = aabb;
				rightAabb.Min = rightAabb.Min.SetValue(axis, position);

				if (leftAabb.Contains(aabb))
					left.Add(item);
				else if (rightAabb.Contains(aabb))
					right.Add(item);
				else if (item is ISliceableSceneGeometry sliceable && sliceable.Complexity > 12)
				{
					ISliceableSceneGeometry sliceLeft = sliceable.Slice(leftAabb);
					ISliceableSceneGeometry sliceRight = sliceable.Slice(rightAabb);

					if (sliceLeft.Complexity + sliceRight.Complexity > sliceable.Complexity * 2)
						leaves.Add(item);
					else
					{
						if (sliceLeft.Complexity > 0)
							left.Add(sliceLeft);

						if (sliceRight.Complexity > 0)
							right.Add(sliceRight);
					}
				}
				else
					leaves.Add(item);
			}

			// Failed to split
			if (left.Count == 0 || right.Count == 0)
			{
				leaves.AddRange(left);
				leaves.AddRange(right);

				left.Clear();
				right.Clear();
			}
		}

		#endregion
	}
}
