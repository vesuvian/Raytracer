﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Raytracer.Geometry;
using Raytracer.Math;

namespace Raytracer.SceneObjects.Geometry.Models
{
    public sealed class ModelSliceSceneGeometry : ISliceableSceneGeometry
    {
        private readonly Mesh m_Mesh;
        private readonly ModelSceneGeometry m_Model;

        public int Complexity { get { return m_Mesh.Triangles.Count; } }

        Vector3 ISceneObject.Position { get => Vector3.Zero; set => throw new NotSupportedException(); }

        Vector3 ISceneObject.Scale { get => Vector3.One; set => throw new NotSupportedException(); }

        Quaternion ISceneObject.Rotation { get => Quaternion.Identity; set => throw new NotSupportedException(); }

        Matrix4x4 ISceneObject.LocalToWorld => Matrix4x4.Identity;

        Matrix4x4 ISceneObject.WorldToLocal => Matrix4x4.Identity;

        Vector3 ISceneObject.Forward => Vector3.UnitZ;

        eRayMask ISceneGeometry.RayMask => m_Model.RayMask;

        public float SurfaceArea { get; }

        public Aabb Aabb { get; }

        public IEnumerable<Intersection> GetIntersections(Ray ray, eRayMask mask,
                                                          float minDelta = float.NegativeInfinity,
                                                          float maxDelta = float.PositiveInfinity)
        {
	        if ((m_Model.RayMask & mask) == eRayMask.None)
		        return Enumerable.Empty<Intersection>();

		        float tMin;
		        float tMax;
		        if (!Aabb.Intersects(ray, out tMin, out tMax))
			        return Enumerable.Empty<Intersection>();

		        if ((tMin < minDelta && tMax < minDelta) ||
		            (tMin > maxDelta && tMax > maxDelta))
			        return Enumerable.Empty<Intersection>();
            return m_Mesh?.GetIntersections(ray, m_Model, m_Model.Material)
                         .Where(i => i.RayDelta >= minDelta && i.RayDelta <= maxDelta)
                   ?? Enumerable.Empty<Intersection>();
        }

        public ISliceableSceneGeometry Slice(Aabb aabb)
        {
            return m_Model.Slice(aabb);
        }

        public ModelSliceSceneGeometry(ModelSceneGeometry model, Aabb aabb)
        {
            m_Model = model;
            m_Mesh = model.Mesh.Clip(model.LocalToWorld, aabb);

            SurfaceArea = m_Mesh.CalculateSurfaceArea();
            Aabb = m_Mesh.CalculateAabb(Matrix4x4.Identity);
        }
    }
}
