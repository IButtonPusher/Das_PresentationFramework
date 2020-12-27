﻿using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.DataBinding;
using Das.Views.Input;

namespace Das.Views.Controls
{
    public interface IButtonBase : IBindableElement,
                                   IInteractiveView,
                                   IInputVisual<InputVisualType>
    {
        IObservableCommand? Command { get; }
    }
}