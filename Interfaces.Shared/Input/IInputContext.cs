﻿using System;
using System.Threading.Tasks;

namespace Das.Views.Input
{
    public interface IInputContext : IInputProvider,
                                     IMouseCaptureManager
    {
        Boolean IsMousePresent { get; }

        Double MaximumFlingVelocity { get; }

        Double MinimumFlingVelocity { get; }

        //Boolean TryCaptureMouseInput(IVisualElement view);

        //Boolean TryReleaseMouseCapture(IVisualElement view);

        //IVisualElement? GetVisualWithMouseCapture();
    }
}