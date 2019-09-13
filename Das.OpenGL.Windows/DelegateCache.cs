using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Das.OpenGL.Windows
{
    public class DelegateCache
    {
        public DelegateCache()
        {
            _cache = new Dictionary<string, Delegate>();
        }

        public const string OpenGL32 = "opengl32.dll";

        [DllImport(OpenGL32, SetLastError = true)]
        public static extern IntPtr wglGetProcAddress(string name);

        public T Get<T>() where T : class
        {
            var delegateType = typeof(T);

            var name = delegateType.Name;

            if (_cache.TryGetValue(name, out var del))
            {
                if (del is T correct)
                    return correct;

                goto fail;
            }

            var proc = wglGetProcAddress(name);
            if (proc == IntPtr.Zero)
                goto fail;
            
            del = Marshal.GetDelegateForFunctionPointer(proc, delegateType);
            
            _cache.Add(name, del);


            return del as T;

            fail:
            throw new Exception("Extension function " + name + " not supported");
        }

        private readonly Dictionary<string, Delegate> _cache;
    }
}
