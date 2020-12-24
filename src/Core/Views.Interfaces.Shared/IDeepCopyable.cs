using System;
using System.Threading.Tasks;

namespace Das.Views
{
    public interface IDeepCopyable<out T>
    {
        T DeepCopy();
    }
}