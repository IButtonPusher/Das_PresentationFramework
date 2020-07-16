//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;


namespace Das.Views.Windows
{
    internal static class DialogsDefaults
    {
        internal static String Caption => "Application";
        internal static String MainInstruction => String.Empty;
        internal static String Content => String.Empty;

        internal const Int32 ProgressBarStartingValue = 0;
        internal const Int32 ProgressBarMinimumValue = 0;
        internal const Int32 ProgressBarMaximumValue = 100;

        internal const Int32 IdealWidth = 0;

        // For generating control ID numbers that won't 
        // collide with the standard button return IDs.
        internal const Int32 MinimumDialogControlId =
            (Int32)TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.Close + 1;
    }
}