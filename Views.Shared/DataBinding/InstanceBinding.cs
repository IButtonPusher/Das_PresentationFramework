using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class InstanceBinding<T> : BaseBinding<T>
    {
        public override IDataBinding<T> DeepCopy()
        {
            return new InstanceBinding<T>();
        }

        public override T GetValue(Object dataContext)
        {
            return dataContext == null ? default! : (T) dataContext;
        }
    }
}