using System;
using System.Threading.Tasks;

namespace Das.Views.Panels;
//public interface IRepeaterPanel<T> : IRepeaterPanel
//{
//    new IBindableElement? Content { get; }
//}

public interface IRepeaterPanel : ISequentialPanel, IContentContainer
{
}