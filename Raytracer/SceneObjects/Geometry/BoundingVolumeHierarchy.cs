using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Math;

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
                m_Aabb ??= Children.Aggregate(new Aabb(), (aabb, child) => aabb + child.Aabb);
                return m_Aabb.Value;
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
        public BoundingVolumeHierarchy(IEnumerable<ISceneGeometry> geometry, int maxDepth = 10)
        {
            m_Leaves = new HashSet<ISceneGeometry>();

            ProcessGeometry(geometry, maxDepth);
        }

        #region Methods

        public IEnumerable<Intersection> GetIntersections(Ray ray, eRayMask mask, float minDelta = float.NegativeInfinity,
                                                          float maxDelta = float.PositiveInfinity)
        {
            if ((RayMask & mask) == eRayMask.None)
                return Enumerable.Empty<Intersection>();

            float tMin;
            float tMax;
            if (!Aabb.Intersects(ray, out tMin, out tMax))
                return Enumerable.Empty<Intersection>();

            if ((tMin < minDelta && tMax < minDelta) ||
                (tMin > maxDelta && tMax > maxDelta))
                return Enumerable.Empty<Intersection>();

            return Children.SelectMany(g => g.GetIntersections(ray, mask, minDelta, maxDelta))
                           .Where(i => i.RayDelta >= minDelta && i.RayDelta <= maxDelta);
        }

        #endregion

        #region Private Methods

        private void ProcessGeometry(IEnumerable<ISceneGeometry> geometry, int maxDepth)
        {
            HashSet<ISceneGeometry> toProcess = geometry.ToHashSet();

            // Depth is zero
            if (maxDepth == 0)
            {
                m_Leaves.AddRange(toProcess);
                return;
            }

            // Find the best way to slice the remaining items in half
            eAxis bestAxis;
            float position = FindSlicePosition(toProcess, out bestAxis);

            // Perform the slice
            HashSet<ISceneGeometry> leaves;
            HashSet<ISceneGeometry> left;
            HashSet<ISceneGeometry> right;
            Slice(toProcess, position, bestAxis, out leaves, out left, out right);

            m_Leaves.AddRange(leaves);

            if (left.Count > 0)
                m_Left = new BoundingVolumeHierarchy(left, maxDepth - 1);

            if (right.Count > 0)
                m_Right = new BoundingVolumeHierarchy(right, maxDepth - 1);
        }

        /// <summary>
        /// Finds the best way to bisect the given items in half.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="bestAxis"></param>
        /// <returns></returns>
        private static float FindSlicePosition(IEnumerable<ISceneGeometry> geometry, out eAxis bestAxis)
        {
            IList<ISceneGeometry> items = geometry as IList<ISceneGeometry> ?? geometry.ToList();

            float bestPosition = 0;
            bestAxis = eAxis.X;

            float bestError = float.MaxValue;

            foreach (eAxis axis in Enum.GetValues(typeof(eAxis)))
            {
                float error;
                float position = FindSlicePosition(items, axis, out error);

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

            float[] edges =
                sorted.SelectMany(g => new[]
                      {
                          g.Aabb.Min.GetValue(axis),
                          g.Aabb.Center.GetValue(axis),
                          g.Aabb.Max.GetValue(axis)
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
                float leafError = leaves.Count / (float)sorted.Length;

                // Surface area should be balanced between nodes
                float leftSurfaceArea = left.Aggregate(0.0f, (area, g) => area + g.SurfaceArea);
                float rightSurfaceArea = right.Aggregate(0.0f, (area, g) => area + g.SurfaceArea);
                float totalSurfaceArea = leftSurfaceArea + rightSurfaceArea;
                float surfaceAreaError =
                    totalSurfaceArea == 0
                    ? 0
                    : MathF.Abs(leftSurfaceArea - rightSurfaceArea) / (leftSurfaceArea + rightSurfaceArea);

                float thisError = leafError + surfaceAreaError;

                // We should have a split
                if (left.Count == 0 || right.Count == 0)
                    thisError += leaves.Count;

                Trace.WriteLine($"{axis} - {position} - {thisError}");

                if (thisError >= error)
                    continue;

                error = thisError;
                bestPosition = position;
            }

            return bestPosition;
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
                else if (item is ISliceableSceneGeometry sliceable)
                {
                    ISceneGeometry sliceLeft = sliceable.Slice(leftAabb);
                    ISceneGeometry sliceRight = sliceable.Slice(rightAabb);
                    left.Add(sliceLeft);
                    right.Add(sliceRight);
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
