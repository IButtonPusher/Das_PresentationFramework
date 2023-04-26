using System;
using System.Threading.Tasks;

namespace Das.Views.Controls;

public interface IVisualSurrogateProvider
{
   Boolean TrySetSurrogate(ref IVisualElement element);
}