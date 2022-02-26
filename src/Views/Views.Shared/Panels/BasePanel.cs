using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Das.Serializer;
using Das.Views.Collections;
using Das.Views.DataBinding;
using Das.Views.Rendering;

namespace Das.Views.Panels
{
   public abstract class BasePanel : BindableElement,
                                     IVisualContainer

   {
      // ReSharper disable once UnusedMember.Global
      protected BasePanel(IVisualBootstrapper visualBootstrapper)
         : this(visualBootstrapper, new VisualCollection())
      {
      }

      protected BasePanel(IVisualBootstrapper visualBootstrapper,
                          IVisualCollection children)
         : base(visualBootstrapper)
      {
         _children = children is VisualCollection good ? good : new VisualCollection(children);
      }

      public IVisualCollection Children => _children;

      public void AddChild(IVisualElement element)
      {
         _children.Add(element);
         InvalidateMeasure();
      }

      public Boolean RemoveChild(IVisualElement element)
      {
         var changed = _children.Remove(element);

         if (changed)
         {
             element.OnParentChanging(default);
             InvalidateMeasure();
         }

         return changed;
      }


      public override Boolean TryHandleInput<TArgs>(TArgs args,
                                                    Int32 x,
                                                    Int32 y)
      {
         //if (!ArrangedBounds.Contains(x, y))
         //   return false;

         for (var c = Children.Count - 1; c >= 0; c--)
         {
            var child = Children[c];
            if (child.ArrangedBounds.Contains(x, y) &&
                child.TryHandleInput(args, x, y))
               return true;
         }

         return TryHandleInput(args);
      }

      public void AddChildren(IEnumerable<IVisualElement> elements)
      {
         _children.AddRange(elements);
         InvalidateMeasure();
      }

      public virtual void OnChildDeserialized(IVisualElement element,
                                              INode node)
      {
      }

      public virtual Boolean Contains(IVisualElement element)
      {
         return _children.IsTrueForAnyChild(element, (child,
                                                      _) =>
         {
            if (child == element)
               return true;

            return child is IVisualContainer container &&
                   container.Contains(element);
         });
      }


      public virtual void AcceptChanges()
      {
         _children.RunOnEachChild(child =>
         {
            if (child is IChangeTracking changeTracking)
               changeTracking.AcceptChanges();
         });

         AcceptChanges(ChangeType.Measure);
         AcceptChanges(ChangeType.Arrange);
      }

      public override void AcceptChanges(ChangeType changeType)
      {
         base.AcceptChanges(changeType);

         _children.RunOnEachChild(changeType, (ct, child) => child.AcceptChanges(ct));
      }

      public virtual Boolean IsChanged =>
         _children.IsTrueForAnyChild(child => child is IChangeTracking {IsChanged: true});

      public override void InvalidateMeasure()
      {
         base.InvalidateMeasure();

         _children.RunOnEachChild(child => child.InvalidateMeasure());
      }

      public override Boolean IsRequiresMeasure
      {
         get => base.IsRequiresMeasure ||
                _children.IsTrueForAnyChild(child => child.IsRequiresMeasure);
         protected set => base.IsRequiresMeasure = value;
      }

      public override Boolean IsRequiresArrange
      {
         get => base.IsRequiresArrange ||
                _children.IsTrueForAnyChild(child => child.IsRequiresArrange);
         protected set => base.IsRequiresArrange = value;
      }

      public override void Dispose()
      {
         base.Dispose();
         _children.Dispose();
      }

      public void AddChildren(params IVisualElement[] elements)
      {
         _children.AddRange(elements);
         InvalidateMeasure();
      }

      /// <summary>
      ///    sealed so inheritors of BasePanel have to override
      /// </summary>
      protected sealed override void OnDataContextChanged(Object? newValue)
      {
         base.OnDataContextChanged(newValue);
         OnDistributeDataContextToChildren(newValue);
      }

      protected virtual void OnDistributeDataContextToChildren(Object? newValue)
      {
      }

      protected readonly VisualCollection _children;
   }
}
