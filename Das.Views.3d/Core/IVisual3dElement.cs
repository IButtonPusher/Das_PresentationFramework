namespace Das.Views.Extended
{
    /// <summary>
    /// A collection of meshes and textures with a common position
    /// </summary>
    public interface IVisual3dElement : I3DElement
    {
        IMesh[] Meshes { get; }
    }
}
