using System;
using System.Reflection;
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
        //public static IPropertyAccessor GetPropertyAccessorOrDie(Type declaringType,
        //                                                         String propertyName)
        //{

        //}
        
        public static PropertyInfo GetTypePropertyOrDie(Type declaringType,
                                                       String propertyName)
        {
            return GetBindingProperty(declaringType, propertyName)
                   ?? GetMissingProperty(declaringType, propertyName);
        }
        
        public static PropertyInfo? GetBindingProperty(Type declaringType,
                                                 String propName)
        {
            if (!propName.Contains(".")) 
                return declaringType.GetProperty(propName);
            
            var subPropTokens = propName.Split('.');
            var propInfo = declaringType.GetProperty(subPropTokens[0]);
            if (propInfo == null)
                return null;

            for (var c = 1; c < subPropTokens.Length; c++)
            {
                propInfo =  propInfo.PropertyType.GetProperty(subPropTokens[c]);
                if (propInfo == null)
                    return null;
            }

            return propInfo;
        }

        private static PropertyInfo GetMissingProperty(Type declaringType,
                                                       String propName)
        {
            throw new MissingMemberException(declaringType.FullName, propName);
        }
        
        public static Object? GetPropertyValue(Type declaringType,
                                               String propName,
                                               Object dataContext)
        {
            if (!propName.Contains("."))
            {
                var prop = GetTypePropertyOrDie(declaringType, propName);
                return prop.GetValue(dataContext, null);
            }

            var subPropTokens = propName.Split('.');
            var propInfo = GetTypePropertyOrDie(declaringType, subPropTokens[0]);

            dataContext = propInfo.GetValue(dataContext, null);

            for (var c = 1; c < subPropTokens.Length; c++)
            {
                propInfo = GetTypePropertyOrDie(propInfo.PropertyType, subPropTokens[c]);
                dataContext = propInfo.GetValue(dataContext, null);
            }

            return dataContext;
        }

        
        
        protected static PropertyInfo GetObjectPropertyOrDie(Object declaringInstance,
                                                       String propertyName)
            => GetTypePropertyOrDie(declaringInstance.GetType(), propertyName);

        protected static readonly Object[] EmptyObjectArray = new Object[0];

        public virtual Boolean IsDataContextBinding => throw new NotImplementedException();

        public abstract Object? GetBoundValue(Object? dataContext);

        public virtual IDataBinding ToSingleBinding()
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateDataContext(Object? dataContext) {}

        public virtual void UpdateSource(Object? source)
        {
            throw new NotImplementedException();
        }

        public virtual IDataBinding Update(Object? dataContext, 
                                           IVisualElement targetVisual)
        {
            UpdateDataContext(dataContext);
            return this;
        }

        public virtual void Evaluate(){}

        public Object? DataContext { get; set; }

        public virtual void Dispose() {}

        public abstract Object Clone();
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