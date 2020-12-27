using System;
using System.Threading.Tasks;

namespace Das.Views.Controls
{
    public interface IVisualSurrogateProvider
    {
        void EnsureSurrogate(ref IVisualElement element);
    }
}