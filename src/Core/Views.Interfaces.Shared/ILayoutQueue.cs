using System;
using System.Collections.Generic;
using System.Text;

namespace Das.Views
{
    public interface ILayoutQueue
    {
        void QueueVisualForMeasure(IVisualElement visual);

        void QueueVisualForArrange(IVisualElement visual);
    }
}
