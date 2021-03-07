using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XamarinAndroidNativeTest
{
    public sealed partial class StrictMode2
    {
        	internal const string TAG = "StrictMode";

        private static readonly bool LOG_V = Android.Util.Log.IsLoggable(TAG, Android.Util.LogPriority
            .Verbose);

        //	private static readonly bool IS_USER_BUILD = "user".Equals(Android.OS.Build.Type);

        //	private static readonly bool IS_ENG_BUILD = "eng".Equals(Android.OS.Build.Type);

        //	public const string VISUAL_PROPERTY = "persist.sys.strictmode.visual";

        //	internal const long MIN_LOG_INTERVAL_MS = 1000;

        //	internal const long MIN_DIALOG_INTERVAL_MS = 30000;

        //	internal const int MAX_SPAN_TAGS = 20;

        //	internal const int MAX_OFFENSES_PER_LOOP = 10;

        //	public const int DETECT_DISK_WRITE = unchecked((int)(0x01));

        //	public const int DETECT_DISK_READ = unchecked((int)(0x02));

        //	public const int DETECT_NETWORK = unchecked((int)(0x04));

        //	public const int DETECT_CUSTOM = unchecked((int)(0x08));

        //	internal const int ALL_THREAD_DETECT_BITS = DETECT_DISK_WRITE | DETECT_DISK_READ 
        //		| DETECT_NETWORK | DETECT_CUSTOM;

        //	public const int DETECT_VM_CURSOR_LEAKS = unchecked((int)(0x200));

        //	public const int DETECT_VM_CLOSABLE_LEAKS = unchecked((int)(0x400));

        //	public const int DETECT_VM_ACTIVITY_LEAKS = unchecked((int)(0x800));

        //	internal const int DETECT_VM_INSTANCE_LEAKS = unchecked((int)(0x1000));

        //	internal const int ALL_VM_DETECT_BITS = DETECT_VM_CURSOR_LEAKS | DETECT_VM_CLOSABLE_LEAKS
        //		 | DETECT_VM_ACTIVITY_LEAKS | DETECT_VM_INSTANCE_LEAKS;

        //	public const int PENALTY_LOG = unchecked((int)(0x10));

        //	public const int PENALTY_DIALOG = unchecked((int)(0x20));

        //	public const int PENALTY_DEATH = unchecked((int)(0x40));

        //	public const int PENALTY_DEATH_ON_NETWORK = unchecked((int)(0x200));

        //	public const int PENALTY_FLASH = unchecked((int)(0x800));

        //	public const int PENALTY_DROPBOX = unchecked((int)(0x80));

        //	public const int PENALTY_GATHER = unchecked((int)(0x100));

        //       public static void incrementExpectedActivityCount (Type klass)
        //       {
        //           ; // Do nothing
        //       }

        //       public static void decrementExpectedActivityCount (Type klass)
        //       {
        //           ; // Do nothing
        //       }

        /// <summary>Enter a named critical span (e.g.</summary>
        /// <remarks>
        /// Enter a named critical span (e.g. an animation)
        /// <p>The name is an arbitary label (or tag) that will be applied
        /// to any strictmode violation that happens while this span is
        /// active.  You must call finish() on the span when done.
        /// <p>This will never return null, but on devices without debugging
        /// enabled, this may return a dummy object on which the finish()
        /// method is a no-op.
        /// <p>TODO: add CloseGuard to this, verifying callers call finish.
        /// </remarks>
        /// <hide></hide>
        public static Span enterCriticalSpan(string name)
        {
            return NO_OP_SPAN;
        }

        private sealed class _NO_OP_SPAN : Span
        {
            //[Sharpen.Stub]
            //[Sharpen.OverridesMethod(@"StrictMode2.Span")]
            public override void finish()
            {
                ; // Do nothing
            }
        }

               private static readonly Span NO_OP_SPAN = new _NO_OP_SPAN ();

        //	internal const int THREAD_PENALTY_MASK = PENALTY_LOG | PENALTY_DIALOG | PENALTY_DEATH
        //		 | PENALTY_DROPBOX | PENALTY_GATHER | PENALTY_DEATH_ON_NETWORK | PENALTY_FLASH;

        //	internal const int VM_PENALTY_MASK = PENALTY_LOG | PENALTY_DEATH | PENALTY_DROPBOX;

        //	private static readonly Java.Util.HashMap<System.Type, int> EMPTY_CLASS_LIMIT_MAP
        //		 = new Java.Util.HashMap<System.Type, int>();

        //	private static volatile int sVmPolicyMask = 0;

        //	private static volatile VmPolicy sVmPolicy;

        //	////[Sharpen.Stub]
        //	private StrictMode2()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public sealed class ThreadPolicy
        //	{
        //		public static readonly ThreadPolicy LAX = new StrictMode2.
        //			.ThreadPolicy(IntPtr.Zero, )

        //		internal readonly int mask;

        //		////[Sharpen.Stub]
        //		private ThreadPolicy(int mask)
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		////[Sharpen.OverridesMethod(@"Java.lang.Object")]
        //		public override string ToString()
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public sealed class Builder
        //		{
        //			private int mMask = 0;

        //			////[Sharpen.Stub]
        //			public Builder()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public Builder(ThreadPolicy policy)
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder detectAll()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder permitAll()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder detectNetwork()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder permitNetwork()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder detectDiskReads()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder permitDiskReads()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder detectCustomSlowCalls()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder permitCustomSlowCalls()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder detectDiskWrites()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder permitDiskWrites()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder penaltyDialog()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder penaltyDeath()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder penaltyDeathOnNetwork()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder penaltyFlashScreen()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder penaltyLog()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy.Builder penaltyDropBox()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			private ThreadPolicy.Builder enable(int bit)
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			private ThreadPolicy.Builder disable(int bit)
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public ThreadPolicy build()
        //			{
        //				throw new System.NotImplementedException();
        //			}
        //		}
        //	}

        //	////[Sharpen.Stub]
        //	public sealed class VmPolicy
        //	{
        //		public static readonly VmPolicy LAX = new StrictMode2
        //			.VmPolicy(0, EMPTY_CLASS_LIMIT_MAP);

        //		internal readonly int mask;

        //		internal readonly Java.Util.HashMap<System.Type, int> classInstanceLimit;

        //		////[Sharpen.Stub]
        //		private VmPolicy(int mask, Java.Util.HashMap<System.Type, int> classInstanceLimit
        //			)
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		////[Sharpen.OverridesMethod(@"Java.lang.Object")]
        //		public override string ToString()
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public sealed class Builder
        //		{
        //			private int mMask;

        //			private Java.Util.HashMap<System.Type, int> mClassInstanceLimit;

        //			private bool mClassInstanceLimitNeedCow = false;

        //			////[Sharpen.Stub]
        //			public Builder()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public Builder(VmPolicy @base)
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder setClassInstanceLimit(System.Type klass
        //				, int instanceLimit)
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder detectActivityLeaks()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder detectAll()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder detectLeakedSqlLiteObjects()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder detectLeakedClosableObjects()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder penaltyDeath()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder penaltyLog()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy.Builder penaltyDropBox()
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			private VmPolicy.Builder enable(int bit)
        //			{
        //				throw new System.NotImplementedException();
        //			}

        //			////[Sharpen.Stub]
        //			public VmPolicy build()
        //			{
        //				throw new System.NotImplementedException();
        //			}
        //		}
        //	}

        //	private sealed class _ThreadLocal_702 : Java.lang.ThreadLocal<Java.Util.ArrayList
        //		<ViolationInfo>>
        //	{
        //		public _ThreadLocal_702()
        //		{
        //		}

        //		////[Sharpen.Stub]
        //		////[Sharpen.OverridesMethod(@"Java.lang.ThreadLocal")]
        //		protected internal override Java.Util.ArrayList<ViolationInfo
        //			> initialValue()
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	private static readonly Java.lang.ThreadLocal<Java.Util.ArrayList<StrictMode2
        //		.ViolationInfo>> gatheredViolations = new _ThreadLocal_702();

        //	////[Sharpen.Stub]
        //	public static void setThreadPolicy(ThreadPolicy policy)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static void setThreadPolicyMask(int policyMask)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static void setBlockGuardPolicy(int policyMask)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static void setCloseGuardEnabled(bool enabled)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	public class StrictModeViolation : dalvik.system.BlockGuard.BlockGuardPolicyException
        //	{
        //		////[Sharpen.Stub]
        //		public StrictModeViolation(int policyState, int policyViolated, string message) : 
        //			base(policyState, policyViolated, message)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	public class StrictModeNetworkViolation : StrictModeViolation
        //	{
        //		////[Sharpen.Stub]
        //		public StrictModeNetworkViolation(int policyMask) : base(policyMask, DETECT_NETWORK
        //			, null)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	private class StrictModeDiskReadViolation : StrictModeViolation
        //	{
        //		////[Sharpen.Stub]
        //		public StrictModeDiskReadViolation(int policyMask) : base(policyMask, DETECT_DISK_READ
        //			, null)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	private class StrictModeDiskWriteViolation : StrictModeViolation
        //	{
        //		////[Sharpen.Stub]
        //		public StrictModeDiskWriteViolation(int policyMask) : base(policyMask, DETECT_DISK_WRITE
        //			, null)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	private class StrictModeCustomViolation : StrictModeViolation
        //	{
        //		////[Sharpen.Stub]
        //		public StrictModeCustomViolation(int policyMask, string name) : base(policyMask, 
        //			DETECT_CUSTOM, name)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	////[Sharpen.Stub]
        //	public static int getThreadPolicyMask()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static ThreadPolicy getThreadPolicy()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static ThreadPolicy allowThreadDiskWrites()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static ThreadPolicy allowThreadDiskReads()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static bool amTheSystemServerProcess()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static bool conditionallyEnableDebugLogging()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void enableDeathOnNetwork()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static int parsePolicyFromMessage(string message)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static int parseViolationFromMessage(string message)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	private sealed class _ThreadLocal_1000 : Java.lang.ThreadLocal<Java.Util.ArrayList
        //		<ViolationInfo>>
        //	{
        //		public _ThreadLocal_1000()
        //		{
        //		}

        //		////[Sharpen.Stub]
        //		////[Sharpen.OverridesMethod(@"Java.lang.ThreadLocal")]
        //		protected internal override Java.Util.ArrayList<ViolationInfo
        //			> initialValue()
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	private static readonly Java.lang.ThreadLocal<Java.Util.ArrayList<StrictMode2
        //		.ViolationInfo>> violationsBeingTimed = new _ThreadLocal_1000();

        //	private sealed class _ThreadLocal_1007 : Java.lang.ThreadLocal<Android.OS.Handler
        //		>
        //	{
        //		public _ThreadLocal_1007()
        //		{
        //		}

        //		////[Sharpen.Stub]
        //		////[Sharpen.OverridesMethod(@"Java.lang.ThreadLocal")]
        //		protected internal override Android.OS.Handler initialValue()
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	private static readonly Java.lang.ThreadLocal<Android.OS.Handler> threadHandler = 
        //		new _ThreadLocal_1007();

        //	////[Sharpen.Stub]
        //	private static bool tooManyViolationsThisLoop()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	//private class AndroidBlockGuardPolicy : Dalvik.SystemInterop.BlockGuard.Policy
        //	//{
        //	//	internal int mPolicyMask;

        //	//	internal readonly Java.Util.HashMap<int, long> mLastViolationTime = new Java.Util.HashMap
        //	//		<int, long>();

        //	//	////[Sharpen.Stub]
        //	//	public AndroidBlockGuardPolicy(int policyMask)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.OverridesMethod(@"Java.lang.Object")]
        //	//	public override string ToString()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.ImplementsInterface(@"dalvik.system.BlockGuard.Policy")]
        //	//	public virtual int getPolicyMask()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.ImplementsInterface(@"dalvik.system.BlockGuard.Policy")]
        //	//	public virtual void onWriteToDisk()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	internal virtual void onCustomSlowCall(string name)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.ImplementsInterface(@"dalvik.system.BlockGuard.Policy")]
        //	//	public virtual void onReadFromDisk()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.ImplementsInterface(@"dalvik.system.BlockGuard.Policy")]
        //	//	public virtual void onNetwork()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	public virtual void setPolicyMask(int policyMask)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	internal virtual void startHandlingViolationException(dalvik.system.BlockGuard.BlockGuardPolicyException
        //	//		 e)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	internal virtual void handleViolationWithTimingAttempt(ViolationInfo
        //	//		 info)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	internal virtual void handleViolation(ViolationInfo info)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}
        //	//}

        //	////[Sharpen.Stub]
        //	private static void executeDeathPenalty(ViolationInfo info)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static void dropboxViolationAsync(int violationMaskSubset, StrictMode2
        //		.ViolationInfo info)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	//private class AndroidCloseGuardReporter : dalvik.system.CloseGuard.Reporter
        //	//{
        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.ImplementsInterface(@"dalvik.system.CloseGuard.Reporter")]
        //	//	public virtual void report(string message, System.Exception allocationSite)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}
        //	//}

        //	////[Sharpen.Stub]
        //	internal static bool hasGatheredViolations()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	internal static void clearGatheredViolations()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void conditionallyCheckInstanceCounts()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	private static long sLastInstanceCountCheckMillis = 0;

        //	private static bool sIsIdlerRegistered = false;

        //	//private sealed class _IdleHandler_1403 : Android.OS.MessageQueue.IIdleHandler
        //	//{
        //	//	public _IdleHandler_1403()
        //	//	{
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.ImplementsInterface(@"Android.OS.MessageQueue.IdleHandler")]
        //	//	public bool queueIdle()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}
        //	//}

        //	private static readonly Android.OS.MessageQueue.IIdleHandler sProcessIdleHandler = 
        //		new _IdleHandler_1403();

        //	////[Sharpen.Stub]
        //	public static void setVmPolicy(VmPolicy policy)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static VmPolicy getVmPolicy()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void enableDefaults()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static bool vmSqliteObjectLeaksEnabled()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static bool vmClosableObjectLeaksEnabled()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void onSqliteObjectLeaked(string message, System.Exception originStack
        //		)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void onWebViewMethodCalledOnWrongThread(System.Exception originStack
        //		)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	private static readonly Java.Util.HashMap<int, long> sLastVmViolationTime = new Java.Util.HashMap
        //		<int, long>();

        //	////[Sharpen.Stub]
        //	public static void onVmPolicyViolation(string message, System.Exception originStack
        //		)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	internal static void writeGatheredViolationsToParcel(Android.OS.Parcel p)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	private class LogStackTrace : System.Exception
        //	{
        //	}

        //	////[Sharpen.Stub]
        //	internal static void readAndHandleBinderCallViolations(Android.OS.Parcel p)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	private static void onBinderStrictModePolicyChange(int newPolicy)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        /// <summary>A tracked, critical time span.</summary>
        /// <remarks>
        /// A tracked, critical time span.  (e.g. during an animation.)
        /// The object itself is a linked list node, to avoid any allocations
        /// during rapid span entries and exits.
        /// </remarks>
        /// <hide></hide>
        public class Span
        {
            private string mName;

            private long mCreateMillis;

            private Span mNext;

            private Span mPrev;

            private readonly ThreadSpanState mContainerState;

            private Span(ThreadSpanState threadState)
            {
                mContainerState = threadState;
            }

            protected internal Span()
            {
                mContainerState = null;
            }

            /// <summary>To be called when the critical span is complete (i.e.</summary>
            /// <remarks>
            /// To be called when the critical span is complete (i.e. the
            /// animation is done animating).  This can be called on any
            /// thread (even a different one from where the animation was
            /// taking place), but that's only a defensive implementation
            /// measure.  It really makes no sense for you to call this on
            /// thread other than that where you created it.
            /// </remarks>
            /// <hide></hide>
            public virtual void finish()
            {
                ThreadSpanState state = mContainerState;
                lock (state)
                {
                    if (mName == null)
                    {
                        return;
                    }

                    if (mPrev != null)
                    {
                        mPrev.mNext = mNext;
                    }

                    if (mNext != null)
                    {
                        mNext.mPrev = mPrev;
                    }

                    if (state.mActiveHead == this)
                    {
                        state.mActiveHead = mNext;
                    }

                    state.mActiveSize--;
                    if (LOG_V)
                    {
                        //Android.Util.Log.d(TAG, "Span finished=" + mName + "; size=" + state.mActiveSize);
                    }

                    this.mCreateMillis = -1;
                    this.mName = null;
                    this.mPrev = null;
                    this.mNext = null;
                    if (state.mFreeListSize < 5)
                    {
                        this.mNext = state.mFreeListHead;
                        state.mFreeListHead = this;
                        state.mFreeListSize++;
                    }
                }
            }
        }

        //[Sharpen.Stub]
        private class ThreadSpanState
        {
            public Span mActiveHead;

            public int mActiveSize;

            public Span mFreeListHead;

            public int mFreeListSize;
        }

        //	//private sealed class _ThreadLocal_1731 : Java.lang.ThreadLocal<StrictMode2
        //	//	.ThreadSpanState>
        //	//{
        //	//	public _ThreadLocal_1731()
        //	//	{
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.OverridesMethod(@"Java.lang.ThreadLocal")]
        //	//	protected internal override ThreadSpanState initialValue()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}
        //	//}

        //	//private static readonly Java.lang.ThreadLocal<ThreadSpanState
        //	//	> sThisThreadSpanState;

        //	//private sealed class _Singleton_1737 : Android.Util.Singleton<Android.view.IWindowManager
        //	//	>
        //	//{
        //	//	public _Singleton_1737()
        //	//	{
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	////[Sharpen.OverridesMethod(@"Android.Util.Singleton")]
        //	//	protected internal override Android.view.IWindowManager create()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}
        //	//}

        //	//private static Android.Util.Singleton<Android.view.IWindowManager> sWindowManager
        //	//	 = new _Singleton_1737();

        //	////[Sharpen.Stub]
        //	public static void noteSlowCall(string name)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void noteDiskRead()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public static void noteDiskWrite()
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	//private static readonly Java.Util.HashMap<System.Type, int> sExpectedActivityInstanceCount
        //	//	 = new Java.Util.HashMap<System.Type, int>();

        //	////[Sharpen.Stub]
        //	public static object trackActivity(object instance)
        //	{
        //		throw new System.NotImplementedException();
        //	}

        //	////[Sharpen.Stub]
        //	public class ViolationInfo
        //	{
        //		public readonly Android.app.ApplicationErrorReport.CrashInfo crashInfo;

        //		public readonly int policy;

        //		public int durationMillis = -1;

        //		public int numAnimationsRunning = 0;

        //		public string[] tags;

        //		public int violationNumThisLoop;

        //		public long violationUptimeMillis;

        //		public string broadcastIntentAction;

        //		public long numInstances = -1;

        //		////[Sharpen.Stub]
        //		public ViolationInfo()
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public ViolationInfo(System.Exception tr, int policy)
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		////[Sharpen.OverridesMethod(@"Java.lang.Object")]
        //		public override int GetHashCode()
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public ViolationInfo(Android.OS.Parcel @in) : this(@in, false)
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public ViolationInfo(Android.OS.Parcel @in, bool unsetGatheringBit)
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public virtual void writeToParcel(Android.OS.Parcel dest, int flags)
        //		{
        //			throw new System.NotImplementedException();
        //		}

        //		////[Sharpen.Stub]
        //		public virtual void dump(Android.Util.LogPrinter pw, string prefix)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	[System.Serializable]
        //	////[Sharpen.Stub]
        //	private class InstanceCountViolation : System.Exception
        //	{
        //		internal readonly System.Type mClass;

        //		internal readonly long mInstances;

        //		internal readonly int mLimit;

        //		////[Sharpen.Stub]
        //		public InstanceCountViolation(System.Type klass, long instances, int limit) : base
        //			(klass.ToString() + "; instances=" + instances + "; limit=" + limit)
        //		{
        //			throw new System.NotImplementedException();
        //		}
        //	}

        //	//////[Sharpen.Stub]
        //	//private sealed class InstanceTracker
        //	//{
        //	//	internal static readonly Java.Util.HashMap<System.Type, int> sInstanceCounts = new 
        //	//		Java.Util.HashMap<System.Type, int>();

        //	//	internal readonly System.Type mKlass;

        //	//	////[Sharpen.Stub]
        //	//	public InstanceTracker(object instance)
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	~InstanceTracker()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}

        //	//	////[Sharpen.Stub]
        //	//	public static int getInstanceCount<_T0>()
        //	//	{
        //	//		throw new System.NotImplementedException();
        //	//	}
        //	//}
        //}
    }
}
