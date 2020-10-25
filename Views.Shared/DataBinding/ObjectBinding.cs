using System;
using System.Threading.Tasks;

namespace Das.Views.DataBinding
{
    public class ObjectBinding<T> : BaseBinding<T>
    {
        public ObjectBinding(T bindingObject)
        {
            _bindingObject = bindingObject;
        }

        public override IDataBinding<T> DeepCopy()
        {
            return new InstanceBinding<T>();
        }

        public override T GetValue(Object? dataContext)
        {
            return _bindingObject;
        }

        private readonly T _bindingObject;
    }
}

//using System;
//using System.Threading.Tasks;
//#if !NET40
//using TaskEx = System.Threading.Tasks.Task;
//#endif

//namespace Das.Views.DataBinding
//{
//    public class ObjectBinding<T> : BaseBinding<T>
//    {
//        public ObjectBinding(T bindingObject)
//        {
//            _bindingObject = bindingObject;
//        }

//        public override IDataBinding<T> DeepCopy()
//        {
//            return new InstanceBinding<T>();
//        }

//        public override T GetValue(Object? dataContext)
//        {
//            return _bindingObject;
//        }

//        private readonly T _bindingObject;
//    }

//    public class ObjectBinding : BaseBinding
//    {
//        public ObjectBinding(Object? bindingObject)
//        {
//            _bindingObject = bindingObject;
//        }

//        public override Object? GetBoundValue(Object? dataContext)
//        {
//            return _bindingObject;
//        }

//        public override Object? GetValue(Object? dataContext)
//        {
//            return dataContext;
//        }

//        public override Task<Object?> GetValueAsync(Object? dataContext)
//        {
//            return TaskEx.FromResult(dataContext);
//        }

//        private readonly Object? _bindingObject;
//    }
//}