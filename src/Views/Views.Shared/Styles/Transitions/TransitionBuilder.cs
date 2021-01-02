using System;
using Das.Views.Core.Drawing;
using Das.Views.Transforms;
using Das.Views.Transitions;

namespace Das.Views.Styles.Transitions
{
    public static class TransitionBuilder
    {
        public static ITransition BuildTransition(IVisualElement visual,
                                                  IDependencyProperty property,
                                                  TimeSpan duration,
                                                  TimeSpan delay,
                                                  TransitionFunctionType fn)
        {
            switch (property)
            {
                case IDependencyProperty<Double> dbl:
                    var r = new DoublePropertyTransition(visual, dbl, duration,
                        delay, fn);
                    return r;

                case IDependencyProperty<IBrush> brush:
                    return new BrushTransition(visual, brush, duration, delay, fn);

                case IDependencyProperty<TransformationMatrix> matrix:
                    return new TransformTransition(visual, matrix, duration, delay, fn);

                default:
                    throw new NotImplementedException();
            }

            
            
        }
    }
}
