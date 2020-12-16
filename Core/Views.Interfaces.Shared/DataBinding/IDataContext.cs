using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public interface IDataContext<T> : IDataContext
    {
        new T Value { get; }
    }

    public interface IDataContext
    {
        Object? Value { get; }
    }
}