using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public interface IDataBinding<T> : IDataBinding,
                                       IDeepCopyable<IDataBinding<T>>
    {
        T GetValue(Object? dataContext);

        Task<T> GetValueAsync(Object? dataContext);
    }

    public interface IDataBinding : IPropertyBinding,
                                    IDisposable,
                                    ICloneable
    {
        /// <summary>
        ///     Returns whether this binding sets the target's DataContext.
        /// </summary>
        Boolean IsDataContextBinding { get; }

        void Evaluate();

        Object? GetBoundValue(Object? dataContext);

        /// <summary>
        ///     Assumes that the binding is to an IEnumerable[T],
        ///     returns the same kind of binding to just T
        /// </summary>
        IDataBinding ToSingleBinding();


        IDataBinding Update(Object? dataContext,
                            IVisualElement targetVisual);

        void UpdateDataContext(Object? dataContext);

        void UpdateSource(Object? source);
    }
}
