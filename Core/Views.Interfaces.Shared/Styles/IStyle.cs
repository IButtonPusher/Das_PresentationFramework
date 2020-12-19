using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public interface IStyle : IEnumerable<AssignedStyle>
    {
        Object? this[StyleSetterType setterType] { get; }

        Object? this[StyleSetterType setterType, 
                     StyleSelector selector] { get; }

        Boolean TryGetValue(StyleSetterType setterType,
                            StyleSelector selector,
                            out Object val);


        void Add(StyleSetterType setterType,
                 StyleSelector selector,
                 Object? value);

        void AddOrUpdate(IStyle style);

        void AddSetter(StyleSetterType setterType,
                       Object? value);
    }
}