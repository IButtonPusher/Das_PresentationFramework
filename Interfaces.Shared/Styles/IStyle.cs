using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Das.Views.Styles
{
    public interface IStyle : IEnumerable<AssignedStyle>
    {
        Object? this[StyleSetter setter] { get; }

        Object? this[StyleSetter setter, StyleSelector selector] { get; }

        Boolean TryGetValue(StyleSetter setter,
                            StyleSelector selector,
                            out Object val);

        Boolean TryGetValue(StyleSetter setter,
                            StyleSelector selector,
                            Object? dataContext,
                            out Object val);

        void Add(StyleSetter setter,
                 StyleSelector selector,
                 Object? value);

        void AddOrUpdate(IStyle style);

        void AddSetter(StyleSetter setter,
                       Object? value);
    }
}