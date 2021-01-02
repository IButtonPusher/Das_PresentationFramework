using System;
using Das.Views.Controls;

namespace Das.Views
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class BaseSurrogateProvider : IVisualSurrogateProvider
    {
        public virtual Boolean TrySetSurrogate(ref IVisualElement element)
        {
            return false;
        }
    }
}
