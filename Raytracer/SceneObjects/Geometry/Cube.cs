using System;
using System.Numerics;
using Raytracer.Math;
using Raytracer.Utils;

namespace Raytracer.SceneObjects.Geometry
{
    public sealed class Cube : AbstractGeometry
    {
	    public override bool GetIntersection(Ray ray, out Intersection intersection)
	    {
		    intersection = default;

			// First transform ray to local space.
		    ray = ray.Multiply(WorldToLocal);

		    float[] xt = CheckAxis(ray.Origin.X, ray.Direction.X);
		    float[] yt = CheckAxis(ray.Origin.Y, ray.Direction.Y);
		    float[] zt = CheckAxis(ray.Origin.Z, ray.Direction.Z);

		    float tMin = MathF.Max(MathF.Max(xt[0], yt[0]), zt[0]);
		    float tMax = MathF.Min(MathF.Min(xt[1], yt[1]), zt[1]);

		    if (tMin > tMax)
			    return false;

			// TODO - Calculate normal
			Vector3 normal = Vector3.Negate(ray.Direction); //* MathUtils.SmoothStep(tMin, tMax, 1.0f);

			// TODO - Calculate position
			Vector3 pos = new Vector3();

			intersection = new Intersection
			{
				Normal = normal,
				Position = pos,
				RayOrigin = ray.Origin
			};

			intersection.Multiply(LocalToWorld);
			return true;
	    }

	    private float[] CheckAxis(float origin, float direction)
	    {

		    float[] t = new float[2];

		    float tMinNumerator = (-1 - origin);
		    float tMaxNumerator = (1 - origin);

		    //Infinities might pop here due to division by zero
		    if (MathF.Abs(direction) >= 0.000001f)
		    {
			    t[0] = tMinNumerator / direction;
			    t[1] = tMaxNumerator / direction;
		    }
		    else
		    {
			    t[0] = tMinNumerator * 1e10f;
			    t[1] = tMaxNumerator * 1e10f;
		    }

		    if (t[0] > t[1])
		    {
			    float temp = t[0];
			    t[0] = t[1];
			    t[1] = temp;
		    }

		    return t;
	    }
	}
}
