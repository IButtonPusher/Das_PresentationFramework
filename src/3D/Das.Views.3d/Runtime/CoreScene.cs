using System.Collections.Generic;

namespace Das.Views.Extended.Runtime;

public class CoreScene : IScene
{
   public IEnumerable<IMesh> VisualElements { get; }

   public CoreScene(IEnumerable<IMesh> visualElements)
   {
      VisualElements = visualElements;
   }
}