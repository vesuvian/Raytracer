using System.Numerics;
using Raytracer.Extensions;
using Raytracer.Materials;
using Raytracer.SceneObjects.Geometry;

namespace Raytracer.Math
{
	public record Intersection
	{
		#region Properties

		public Vector3 Position { get; init; }
		public Vector3 Normal { get; init; }
		public Vector3 Tangent { get; init; }
		public Vector3 Bitangent { get; init; }
		public Ray Ray { get; init; }
		public Vector2 Uv { get; init; }
        public ISceneGeometry Geometry { get; init; }
        public IMaterial Material { get; init; }

        public float FaceRatio { get { return Vector3.Dot(Ray.Direction, Normal); } }

		public float Distance { get { return (Position - Ray.Origin).Length(); } }

		public float RayDelta
		{
			get
			{
				bool inFront = Vector3.Dot(Vector3.Normalize(Position - Ray.Origin), Ray.Direction) > 0;
				return inFront ? Distance : -Distance;
			}
		}

        #endregion

		#region Methods

		public Intersection Multiply(Matrix4x4 matrix)
		{
			return new Intersection
			{
				Position = matrix.MultiplyPoint(Position),
				Normal = matrix.MultiplyNormal(Normal),
				Tangent = Vector3.Normalize(matrix.MultiplyDirection(Tangent)),
				Bitangent = Vector3.Normalize(matrix.MultiplyDirection(Bitangent)),
				Ray = Ray.Multiply(matrix),
				Uv = Uv,
				Geometry = Geometry,
				Material = Material
			};
		}

		public Intersection Flip()
		{
			return new Intersection
			{
				Position = Position,
				Normal = Normal * -1,
				Tangent = Tangent * -1,
				Bitangent = Bitangent,
				Ray = Ray,
				Uv = Uv,
                Geometry = Geometry,
                Material = Material
			};
		}

		#endregion
	}
}
