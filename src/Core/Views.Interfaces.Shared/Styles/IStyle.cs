using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public interface IStyle : IEnumerable<AssignedStyle>
    {
        Object? this[StyleSetterType setterType] { get; }

        Object? this[StyleSetterType setterType, 
                     VisualStateType type] { get; }

        Boolean TryGetValue(StyleSetterType setterType,
                            VisualStateType type,
                            out Object val);


        void Add(StyleSetterType setterType,
                 VisualStateType type,
                 Object? value);

        //void AddOrUpdate(IStyle style);

        //void AddSetter(StyleSetterType setterType,
        //               Object? value);
    }
}