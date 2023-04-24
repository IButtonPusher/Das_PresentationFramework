using System;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;

namespace Das.OpenGL;

/// <summary>
///     Manages the window used for rendering -  including needed size changes
///     Provides access to device contexts used to render.
/// </summary>
// ReSharper disable once InconsistentNaming
public interface IGLContext
{
   IntPtr DeviceContextHandle { get; }

   IntPtr RenderContextHandle { get; }

   ISize Size { get; }

   void EnsureSurfaceSize();

   void Flush();

   void Initialize();
}