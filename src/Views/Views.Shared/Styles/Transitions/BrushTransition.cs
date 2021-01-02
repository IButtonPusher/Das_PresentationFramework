using System;
using System.Threading.Tasks;
using Das.Views.Core.Drawing;
using Das.Views.Transitions;

namespace Das.Views.Styles.Transitions
{
    public class BrushTransition : PropertyTransition<IBrush>
    {
        public BrushTransition(IVisualElement visual,
                               IDependencyProperty<IBrush> property,
                               TimeSpan duration,
                               TimeSpan delay,
                               TransitionFunctionType timing) : base(visual, property, duration, delay, timing)
        {
            _startValue = SolidColorBrush.Tranparent;
            _endValue = SolidColorBrush.Tranparent;
        }

        public override void SetValue(IBrush startValue,
                                      IBrush endValue)
        {
            switch (startValue)
            {
                case SolidColorBrush scb:
                    _startValue = scb;
                    break;

                case null:
                    _startValue = SolidColorBrush.Tranparent;
                    break;

                default:
                    throw new NotSupportedException();
            }

            switch (endValue)
            {
                case SolidColorBrush scb:
                    _endValue = scb;
                    break;

                case null:
                    _endValue = SolidColorBrush.Tranparent;
                    break;

                default:
                    throw new NotSupportedException();
            }


            _startA = _startValue.Color.A;
            _startR = _startValue.Color.R;
            _startG = _startValue.Color.G;
            _startB = _startValue.Color.B;

            _alphaDiff = _endValue.Color.A - _startA;
            _rDiff = _endValue.Color.R - _startR;
            _gDiff = _endValue.Color.G - _startG;
            _bDiff = _endValue.Color.B - _startB;

            base.SetValue(startValue!, endValue!);
        }

        protected override IBrush GetCurrentValue(Double runningPct)
        {
            var a = Convert.ToByte(_startA + _alphaDiff * runningPct);
            var r = Convert.ToByte(_startR + _rDiff * runningPct);
            var g = Convert.ToByte(_startG + _gDiff * runningPct);
            var b = Convert.ToByte(_startB + _bDiff * runningPct);

            return SolidColorBrush.FromArgb(a, r, g, b);
        }

        private Double _alphaDiff;
        private Double _bDiff;
        private SolidColorBrush _endValue;
        private Double _gDiff;
        private Double _rDiff;

        private Double _startA;
        private Double _startB;
        private Double _startG;
        private Double _startR;

        private SolidColorBrush _startValue;
    }
}