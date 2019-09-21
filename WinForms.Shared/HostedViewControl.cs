﻿using Das.Views.Panels;
using Das.Views.Styles;
using System;
using Das.Views;
using Das.Views.Core.Geometry;
using Das.Views.Rendering;
using Das.Views.Winforms;


namespace WinForms.Shared
{
    public abstract class HostedViewControl : HostedControl, 
        IBoundElementContainer, IWindowsViewHost
    {
        protected HostedViewControl(IView view, IStyleContext styleContext)
            : this(styleContext)
        {
            View = view;
        }

        protected HostedViewControl(IStyleContext styleContext)
        {
            StyleContext = styleContext;
            _zoomLevel = 1;
        }
     

        public event EventHandler DataContextChanged;

        private Boolean _isChanged;
        private IViewModel _dataContext;

        public IView View { get; protected set; }

        public IVisualElement Element { get; set; }
        public IViewModel DataContext
        {
            get => _dataContext;
            set
            {
                _dataContext = value;
                View.SetDataContext(value);
                _isChanged = true;
                DataContextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetView(IView view) => View = view;

        public IStyleContext StyleContext { get; set; }

        public Thickness RenderMargin { get; } = Thickness.Empty;

        private Double _zoomLevel;

        public Double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                _isChanged = true;
            }
        }

        public T GetStyleSetter<T>(StyleSetters setter, IVisualElement element)
            => StyleContext.GetStyleSetter<T>(setter, element);

        public void AcceptChanges()
        {
            
        }

        public virtual Boolean IsChanged => _isChanged;
    }
}
