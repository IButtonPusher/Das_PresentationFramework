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

        public override T GetValue(Object? dataContext)
        {
            return dataContext == null ? default! : (T) dataContext;
        }

        public override void Dispose()
        {
            
        }
    }
}

//using System;
//using System.Threading.Tasks;
//#if !NET40
//using TaskEx = System.Threading.Tasks.Task;
//#endif


//namespace Das.Views.DataBinding
//{
//    public class InstanceBinding<T> : BaseBinding<T>
//    {
//        public override IDataBinding<T> DeepCopy()
//        {
//            return new InstanceBinding<T>();
//        }

//        public override T GetValue(Object? dataContext)
//        {
//            return dataContext == null 
//                ? default! 
//                : (T) dataContext;
//        }
//    }

//    public class InstanceBinding : BaseBinding
//    {
//        public override Object? GetBoundValue(Object? dataContext)
//        {
//            return dataContext;
//        }

//        public override Object? GetValue(Object? dataContext)
//        {
//            return dataContext;
//        }

//        public override Task<Object?> GetValueAsync(Object? dataContext)
//        {
//            return TaskEx.FromResult(dataContext);
//        }
//    }
//}