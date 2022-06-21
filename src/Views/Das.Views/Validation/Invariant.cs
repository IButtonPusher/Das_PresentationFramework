using System;
using System.Threading.Tasks;
using Das.Views.Localization;

namespace Das.Views.Validation
{
    public static class Invariant
    {
        public static void Assert(Boolean condition)
        {
            if (condition)
                return;
            FailFast(null, null);
        }

        public static void Assert(Boolean condition,
                                  String invariantMessage)
        {
            if (condition)
                return;
            FailFast(invariantMessage, null);
        }

        private static void FailFast(String? message,
                                     String? detailMessage)
        {
            //if (Invariant.IsDialogOverrideEnabled)
            //    Debugger.Break();
            Environment.FailFast(SR.Get("InvariantFailure"));
        }

        public static Boolean Strict
        {
            get => _strict;
            set => _strict = value;
        }

        private static Boolean _strict;
    }
}
