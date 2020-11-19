﻿using System;
using System.Threading.Tasks;
using Das.ViewModels;
using Das.Views.Core.Geometry;
using Das.Views.Input;
using Das.Views.Panels;
using Das.Views.Rendering;
using Das.Views.Rendering.Geometry;
using Das.Views.Styles;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Das.Views.Controls
{
    public abstract class ButtonBase<T> : ContentPanel<T>,
                                 IHandleInput<MouseClickEventArgs>,
                                 IHandleInput<MouseDownEventArgs>,
                                 IHandleInput<MouseUpEventArgs>,
                                 IHandleInput<MouseOverEventArgs>,
                                 IButtonBase
    {
        private readonly IVisualBootStrapper _visualBootStrapper;

        protected ButtonBase(IVisualBootStrapper visualBootStrapper)
        : base(visualBootStrapper)
        {
            _visualBootStrapper = visualBootStrapper;
            _currentStyleSelector = StyleSelector.None;
        }

        public override void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext)
        {
            var contentCanHave = GetPaddedSpace(renderContext, 
                availableSpace, out var padding);
            var contentRect = new ValueRenderRectangle(padding.Left, padding.Top, contentCanHave,
                availableSpace.Offset);

            if (!(Content is { } content))
            {
                if (Value is { } valid && ContentTemplate is { } template)
                    renderContext.DrawElement(template.Template, contentRect);
                return;
            }

            renderContext.DrawElement(content, contentRect);
        }

        //public override void Dispose()
        //{
        //}

        public override ValueSize Measure(IRenderSize availableSpace,
                                          IMeasureContext measureContext)
        {
            var contentCanHave = GetPaddedSpace(measureContext, 
                availableSpace, out var padding);

            if (!(Content is { } content))
            {
                if (Value is { } valid && ContentTemplate is { } template)
                {
                    var res = measureContext.MeasureElement(template.Template, contentCanHave);
                    return padding.IsEmpty ? res : res + padding;
                }

                return ValueSize.Empty;
            }

            var contentWants = measureContext.MeasureElement(content, contentCanHave);
            var ambition = contentWants + padding;
            if (ambition.Width > availableSpace.Width ||
                ambition.Height > availableSpace.Height)
                return availableSpace.ToValueSize();
            return contentWants + padding;
        }

        private IRenderSize GetPaddedSpace(IStyleProvider styleContext,
                                           IRenderSize availableSpace,
                                           out Thickness padding)
        {
            padding = styleContext.GetStyleSetter<Thickness>(StyleSetter.Padding,
                CurrentStyleSelector, this);

            return padding.IsEmpty
                ? availableSpace
                : availableSpace.Reduce(padding);
        }

        public StyleSelector CurrentStyleSelector
        {
            get => _currentStyleSelector;
            //set => SetValue(ref _currentStyleSelector, value, OnCurrentSelectorChanged);
        }

        protected void AddStyleSelector(StyleSelector value)
        {
            var val = _currentStyleSelector == StyleSelector.None 
            ? value : 
            _currentStyleSelector | value;

            SetValue(ref _currentStyleSelector, val, OnCurrentSelectorChanged,
                nameof(CurrentStyleSelector));
        }

        protected void RemoveStyleSelector(StyleSelector value)
        {
            var val = _currentStyleSelector & ~value;
            
            if (val == 0)
            {}

            SetValue(ref _currentStyleSelector, val, OnCurrentSelectorChanged,
                nameof(CurrentStyleSelector));
        }

        public virtual Boolean OnInput(MouseClickEventArgs args)
        {
            if (ClickMode != ClickMode.Release || !(Command is {} cmd))
                return true;

            //todo: this is not good
            var boundValue = BoundValue ?? Binding?.GetBoundValue(DataContext);

            cmd.ExecuteAsync(boundValue!).ConfigureAwait(false);
            return true;
        }

        InputAction IInteractiveView.HandlesActions => InputAction.LeftMouseButtonDown |
                                                       InputAction.LeftMouseButtonUp |
                                                       InputAction.LeftClick |
                                                       InputAction.MouseOver;

        public virtual Boolean OnInput(MouseDownEventArgs args)
        {
            args.InputContext.TryCaptureMouseInput(this);

            AddStyleSelector(StyleSelector.Active);

            if (ClickMode != ClickMode.Press || !(Command is {} cmd))
                return true;

            cmd.ExecuteAsync(BoundValue).ConfigureAwait(false);
            return true;
        }

        public virtual Boolean OnInput(MouseOverEventArgs args)
        {
            if (args.IsMouseOver)
                AddStyleSelector(StyleSelector.Hover);
            else
                RemoveStyleSelector(StyleSelector.Hover);

            return true;
        }

        public virtual Boolean OnInput(MouseUpEventArgs args)
        {
            RemoveStyleSelector(StyleSelector.Active);
            args.InputContext.TryReleaseMouseCapture(this);
            return true;
        }

        public ClickMode ClickMode
        {
            get => _clickMode;
            set => SetValue(ref _clickMode, value);
        }

        public IObservableCommand<T>? Command { get; set; }

        private void OnCurrentSelectorChanged(StyleSelector value)
        {
            InvalidateArrange();

            IsChanged = true;
        }


        private ClickMode _clickMode;
        private StyleSelector _currentStyleSelector;
    }
}