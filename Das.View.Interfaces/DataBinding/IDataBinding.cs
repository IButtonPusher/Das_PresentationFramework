﻿using System;

namespace Das.Views.DataBinding
{
    public interface IDataBinding<T> : IDataBinding, IDeepCopyable<IDataBinding<T>>
    {
        T GetValue(Object dataContext);

        //void SetValue(T value);

        //T GetLastValue();
    }

    public interface IDataBinding
    {
        Object GetBoundValue(Object dataContext);

        /// <summary>
        /// Assumes that the binding is to an IEnumerable[T],
        /// returns the same kind of binding to just T
        /// </summary>
        /// <returns></returns>
        IDataBinding ToSingleBinding();

        //Object GetLastBoundValue();
    }
}