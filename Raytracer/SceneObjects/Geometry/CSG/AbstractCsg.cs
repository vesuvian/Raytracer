namespace Raytracer.SceneObjects.Geometry.CSG
{
	public abstract class AbstractCsg : AbstractSceneGeometry
	{
		public ISceneGeometry A { get; set; }
		public ISceneGeometry B { get; set; }

		protected override float CalculateUnscaledSurfaceArea()
		{
			return A.SurfaceArea + B.SurfaceArea;
		}
	}
}
