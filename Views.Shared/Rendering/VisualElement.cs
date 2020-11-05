﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Das.Views.Core.Geometry;
using Das.Views.Mvvm;
using Das.Views.Panels;

namespace Das.Views.Rendering
{
    public abstract class VisualElement : NotifyPropertyChangedBase,
                                          IVisualElement
    {
        protected VisualElement()
        {
            Id = Interlocked.Increment(ref _currentId);
        }

        public abstract ISize Measure(IRenderSize availableSpace,
                                      IMeasureContext measureContext);

        public abstract void Arrange(IRenderSize availableSpace,
                                     IRenderContext renderContext);

        public virtual IVisualElement DeepCopy()
        {
            var newObject = (VisualElement) Activator.CreateInstance(GetType());
            newObject.Id = Id;
            return newObject;
        }

        public Int32 Id { get; private set; }

        public event Action<IVisualElement>? Disposed;

        public virtual void OnParentChanging(IContentContainer? newParent)
        {
            
        }

        public override void Dispose()
        {
            base.Dispose();

            Disposed?.Invoke(this);

            Disposed = null;
        }

        public virtual Boolean IsEnabled
        {
            get => _isEnabled;
            set => SetValue(ref _isEnabled, value);
        }

        private static Int32 _currentId;


        private Boolean _isEnabled;
    }
}