using System;

namespace Das.Views
{
    public interface IDeepCopyable<out T>
    {
        T DeepCopy();
    }
}