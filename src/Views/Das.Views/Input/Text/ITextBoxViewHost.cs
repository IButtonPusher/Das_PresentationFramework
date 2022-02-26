using System;
using System.Threading.Tasks;

namespace Das.Views.Input.Text
{
    public interface ITextBoxViewHost
    {
        ITextContainer TextContainer { get; }

        Boolean IsTypographyDefaultValue { get; }
    }
}
