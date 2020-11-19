using System;
using System.Threading.Tasks;

namespace Das.Views.Controls
{
    // ReSharper disable once UnusedType.Global
    public class Button<T> : ButtonBase<T>
    {
        public Button(IVisualBootStrapper templateResolver) : base(templateResolver)
        {
        }
    }

    public class Button : ButtonBase
    {
        public Button(IVisualBootStrapper templateResolver) : base(templateResolver)
        {
        }
    }
}