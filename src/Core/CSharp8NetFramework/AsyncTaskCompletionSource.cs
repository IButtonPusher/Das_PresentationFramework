using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public class AsyncTaskCompletionSource<T> : TaskCompletionSource<T>
    {
        public AsyncTaskCompletionSource() 
            : base(_isRunContinuationsAsynchronously 
            ? (TaskCreationOptions) 64
            : TaskCreationOptions.None)
        {
            
        }

        static AsyncTaskCompletionSource()
        {
            var strType = typeof(String);
            var assemblyUri = strType.Assembly.CodeBase;
            var versionInfo = FileVersionInfo.GetVersionInfo(new Uri(assemblyUri).LocalPath);
            _isRunContinuationsAsynchronously = versionInfo.FileMajorPart >= 4 &&
                                                versionInfo.FileMinorPart >= 5;
        }

        private static readonly Boolean _isRunContinuationsAsynchronously;
        
    }
}
