using System;
using System.Threading.Tasks;
using Das.Views.DataBinding;
using Das.Views.Input.Text.Pointers;
using Das.Views.Input.Text.Tree;

namespace Das.Views.Input.Text
{
    public interface ITextElement : IBindableElement
    {
        void BeforeLogicalTreeChange();

        void Reposition(ITextPointer start,
                        ITextPointer end);

        void OnTextUpdated();

        Int32 IMELeftEdgeCharCount { get; }

        TextTreeTextElementNode TextElementNode { get; set; }

        bool IsFirstIMEVisibleSibling { get; }

        void AfterLogicalTreeChange();
    }
}
