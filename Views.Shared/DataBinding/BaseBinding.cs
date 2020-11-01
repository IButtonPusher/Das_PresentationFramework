using System;
using System.Threading.Tasks;

#if !NET40
using TaskEx = System.Threading.Tasks.Task;
#endif

namespace Das.Views.DataBinding
{
    public abstract class BaseBinding<T> : BaseBinding, 
                                           IDataBinding<T>
    {
        public abstract T GetValue(Object? dataContext);

        public virtual Task<T> GetValueAsync(Object? dataContext)
        {
            var val = GetValue(dataContext);
            return TaskEx.FromResult(val);
        }

        public override Object? GetBoundValue(Object? dataContext)
        {
            return GetValue(dataContext);
        }

        public abstract IDataBinding<T> DeepCopy();
    }

    public abstract class BaseBinding : IDataBinding
    {
        public abstract Object? GetBoundValue(Object? dataContext);

        public virtual IDataBinding ToSingleBinding()
        {
            throw new NotImplementedException();
        }

        public Object? DataContext { get; set; }
    }
}

//using System;
//using System.Threading.Tasks;

//#if !NET40
//using TaskEx = System.Threading.Tasks.Task;
//#endif

//namespace Das.Views.DataBinding
//{
//    public abstract class BaseBinding<T> : //BaseBinding, 
//                                           IDataBinding<T>
//    {
//        public abstract T GetValue(Object? dataContext);

//        public virtual Task<T> GetValueAsync(Object? dataContext)
//        {
//            var val = GetValue(dataContext);
//            return TaskEx.FromResult(val);
//        }

//        //public override Object? GetBoundValue(Object dataContext)
//        //{
//        //    return GetValue(dataContext);
//        //}

//        public abstract IDataBinding<T> DeepCopy();

//        public Object? DataContext { get; set; }
//    }

//    public abstract class BaseBinding : IDataBinding
//    {
//        public abstract Object? GetBoundValue(Object? dataContext);

//        public virtual IDataBinding ToSingleBinding()
//        {
//            throw new NotImplementedException();
//        }

//        public abstract Object? GetValue(Object? dataContext);

//        public abstract Task<Object?> GetValueAsync(Object? dataContext);
        

//        public Object DataContext { get; set; }
//    }
//}


////using System;
////using System.Threading.Tasks;
////#if !NET40
////using TaskEx = System.Threading.Tasks.Task;

////#endif

////namespace Das.Views.DataBinding
////{
////    public abstract class BaseBinding<T> : //BaseBinding, 
////                                           IDataBinding<T>
////    {
////        public abstract IDataBinding ToSingleBinding();

////        Object? IDataBinding.GetValue(Object? dataContext)
////        {
////            return GetValue(dataContext);
////        }

////        async Task<Object?> IDataBinding.GetValueAsync(Object? dataContext)
////        {
////            return await GetValueAsync(dataContext);
////        }

////        public abstract T GetValue(Object? dataContext);

////        public virtual Task<T> GetValueAsync(Object? dataContext)
////        {
////            var val = GetValue(dataContext);
////            return TaskEx.FromResult(val);
////        }

////        public virtual Object? GetBoundValue(Object? dataContext)
////        {
////            return GetValue(dataContext);
////        }

////        public abstract IDataBinding<T> DeepCopy();
////    }

////    public abstract class BaseBinding : IDataBinding
////    {
////        public abstract Object? GetBoundValue(Object? dataContext);

////        public virtual IDataBinding ToSingleBinding()
////        {
////            throw new NotImplementedException();
////        }

////        public abstract Object? GetValue(Object? dataContext);

////        public virtual Task<Object?> GetValueAsync(Object? dataContext)
////        {
////            var val = GetValue(dataContext);
////            return TaskEx.FromResult(val);
////        }

////        public Object? DataContext { get; set; }
////    }
////}