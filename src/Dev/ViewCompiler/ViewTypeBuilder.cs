using System;
using System.Reflection;
using System.Reflection.Emit;
using Das.Serializer;
using Das.Serializer.Objects;
using Das.Views.DevKit;
using Das.Views.Panels;
using Das.Views.Rendering;

namespace ViewCompiler
{
    public class ViewTypeBuilder
    {
        private readonly IViewDeserializer _serializer;
        private readonly IObjectManipulator _objectManipulator;
        private readonly ISerializationDepth _settings;
        private readonly DasCodeGenerator _codeGenerator;

        public ViewTypeBuilder(IViewDeserializer serializer, 
            IObjectManipulator objectManipulator, ISerializationDepth settings)
        {
            _serializer = serializer;
            _objectManipulator = objectManipulator;
            _settings = settings;
            _codeGenerator = new DasCodeGenerator("ViewTypes", "ViewsModule", AssemblyBuilderAccess.RunAndSave);
        }

        public Type BuildViewType(ViewBuilder viewBuilder, 
                                  String viewName)
        {
            var typeBuilder = GetBuilder(viewBuilder, viewName);
            var il = OpenConstructor(typeBuilder);


            IVisualRenderer current = viewBuilder.Content;
            var bldr = Reconstruct(current, il);
            SetContent(typeof(IContentContainer), bldr, il);

            il.Emit(OpCodes.Ret);

            var bob = typeBuilder.CreateType();
            return bob;
        }

        private TypeBuilder GetBuilder(ViewBuilder viewBuilder, String viewName)
        {
            var bindingType = _serializer.TypeInferrer.GetTypeFromClearName(viewBuilder.Binding);
            //var viewInterface = typeof(View<>).MakeGenericType(bindingType);

            var typeBuilder = _codeGenerator.GetTypeBuilder(viewName);
            typeBuilder.SetParent(typeof(View));
            return typeBuilder;
        }

        private static ILGenerator OpenConstructor(TypeBuilder typeBuilder)
        {
            var ctor = typeBuilder.DefineConstructor(
                MethodAttributes.Public | MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);
            var il = ctor.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0); //constructing object
            return il;
        }

        private LocalBuilder Reconstruct(IVisualRenderer current, ILGenerator il)
        {
            switch (current)
            {
                case IVisualContainer visualContainer
                when visualContainer.Children.Count > 0:
                    var pnlLocalVar = BuildVisualContainer(visualContainer, il);
                    return pnlLocalVar;
                case IContentContainer contentHost:
                    var hostLocalVar = BuildContentContainer(contentHost, il);
                    return hostLocalVar;
                default:
                    var bldr = Imitate(current, il);
                    return bldr;
            }
        }

        private LocalBuilder BuildVisualContainer(IVisualContainer visualContainer, ILGenerator il)
        {
            var cType = visualContainer.GetType();
            var pnlLocalVar = Construct(cType, il);
            var addChildMeth = cType.GetMethod(
                nameof(IVisualContainer.AddChild));
            if (addChildMeth == null)
                throw new InvalidOperationException("Can't add child to " + cType);

            //foreach (var child in visualContainer.Children)
            visualContainer.Children.RunOnEachChild(child => 
            {
                var childLocalVar = Reconstruct(child, il);
                il.Emit(OpCodes.Ldloc, pnlLocalVar);
                il.Emit(OpCodes.Ldloc, childLocalVar);
                il.Emit(OpCodes.Callvirt, addChildMeth);
            });

            return pnlLocalVar;
        }

        private LocalBuilder BuildContentContainer(IContentContainer container, ILGenerator il)
        {
            //instantiate an object of the .Content's type
            var contentLocalVar = Reconstruct(container.Content, il);

            //instaniate an object of the content container
            var contType = container.GetType();
            var contBldr = Construct(contType, il);

            //set contentHost.Content = val;
            SetContent(contBldr, contentLocalVar, il);

            return contBldr;
        }

        private static void SetContent(LocalBuilder recipient, 
            LocalBuilder value, ILGenerator il)
        {
            il.Emit(OpCodes.Ldloc, recipient);
            SetContent(recipient.LocalType, value, il);
        }
        

        private static void SetContent(Type recipientType, LocalBuilder value, 
            ILGenerator il)
        {
            var setter = recipientType?.GetProperty("Content")?.GetSetMethod();
            if (setter == null)
                throw new InvalidOperationException(
                    "Can't set Content on object of type " + recipientType);

            il.Emit(OpCodes.Ldloc, value);
            il.Emit(OpCodes.Callvirt, setter);
        }

        private static void SetPropertyValue(LocalBuilder recipient, 
                                             IProperty fieldInfo,
            LocalBuilder value, ILGenerator il)
        {
            var setter = recipient.LocalType?.GetProperty(fieldInfo.Name)?.GetSetMethod(true);
            if (setter == null)
                throw new InvalidOperationException(
                    "Can't set Content on object of type " + fieldInfo.Type);

            il.Emit(OpCodes.Ldloc, recipient);
            il.Emit(OpCodes.Ldloc, value);
            
            il.Emit(OpCodes.Callvirt, setter);
        }

        private LocalBuilder Imitate(Object obj, ILGenerator il)
        {
            var type = obj.GetType();
            var bldr = Construct(type, il);

            var node = new ValueNode(obj);

            var props =_objectManipulator.GetPropertyResults(node, _settings);
            
            foreach (var prop in props)
            {
                LocalBuilder? liveLocal = null;

                switch (prop.Value)
                {
                    case String str:
                        il.Emit(OpCodes.Ldstr, str);
                        liveLocal = il.DeclareLocal(typeof(String));
                        break;
                    case Int32 i32:
                        il.Emit(OpCodes.Ldind_I4, i32);
                        liveLocal = il.DeclareLocal(typeof(Int32));
                        break;
                }

                if (liveLocal != null)
                {
                    il.Emit(OpCodes.Stloc, liveLocal);
                    SetPropertyValue(bldr, prop, liveLocal, il);
                }
            }

            return bldr;
        }

        private static LocalBuilder Construct(Type type, ILGenerator il)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
                throw new InvalidOperationException("Type: " + type + " lacks a default constructor");
            il.Emit(OpCodes.Newobj, ctor);
            var bldr = il.DeclareLocal(type);
            il.Emit(OpCodes.Stloc, bldr);
            return bldr;
        }

//        public void Save()
//        {
//            _codeGenerator.Save("bob.dll");
//        }
    }
}
