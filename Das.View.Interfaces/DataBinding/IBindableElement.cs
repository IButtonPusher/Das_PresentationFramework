﻿using System;
using Das.Views.Rendering;

namespace Das.Views.DataBinding
{
    public interface IBindableElement : IVisualElement, IDataContext, IBindingSetter
    {
        IDataBinding Binding { get; set; }

        Object DataContext { get; set; }

        Object GetBoundValue(Object dataContext);
    }

    public interface IBindableElement<T> : IBindableElement
    {
        new T GetBoundValue(Object dataContext);

        void SetBoundValue(T value);

        new IDataBinding<T> Binding { get; set; }
    }
}