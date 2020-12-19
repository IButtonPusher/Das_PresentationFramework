using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;

namespace Das.Views.Styles
{
    public class ElementStyle : Style
    {
        private readonly IStyleContext _styleContext;

        public ElementStyle(IVisualElement element,
                            IStyleContext styleContext)
        {
            _styleContext = styleContext;
            Element = element;
        }

        protected override void UpdateTransition(AssignedStyle style)
        {
            //Debug.WriteLine("Updating transition");

            base.UpdateTransition(style);
            Element.InvalidateMeasure();
            _styleContext.CoerceIsChanged();
        }

        public override void Add(StyleSetterType setterType, 
                                 StyleSelector selector, 
                                 Object? value)
        {
            var key = new AssignedStyle(setterType, selector, value);

            if (setterType == StyleSetterType.Transition)
            {
                var transitions = value as Transition[] 
                                  ?? throw new ArgumentOutOfRangeException(nameof(value));

                foreach (var trans in transitions)
                {
                    var tranqui = new AssignedStyle(trans.SetterType, selector, trans);
                    _transitions[tranqui] = trans;
                }
            }
            else if (_transitions.TryGetValue(key, out var transition))
            {
                if (!_setters.TryGetValue(key, out var existed))
                    existed = null;

                switch (value)
                {
                    case Thickness thickness:
                        var runningThickness = new ThicknessTransition(existed, transition, key, UpdateTransition);
                        runningThickness.Start();
                        break;

                    case Double dbl:
                        var running = new DoubleTransition(Element, existed,
                            transition, this, key, UpdateTransition);
                        running.Start();
                        break;

                    default:
                        throw new NotImplementedException();
                }

                
            }
            else base.Add(setterType, selector, value);
        }

        public IVisualElement Element { get; }
    }
}