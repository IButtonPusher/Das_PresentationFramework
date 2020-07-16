//Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Threading.Tasks;

namespace Das.Views.Windows
{
    /// <summary>
    ///     HRESULT Wrapper
    /// </summary>
    public enum HResult
    {
        /// <summary>
        ///     S_OK
        /// </summary>
        Ok = 0x0000,

        /// <summary>
        ///     S_FALSE
        /// </summary>
        False = 0x0001,

        /// <summary>
        ///     E_INVALIDARG
        /// </summary>
        InvalidArguments = unchecked((Int32) 0x80070057),

        /// <summary>
        ///     E_OUTOFMEMORY
        /// </summary>
        OutOfMemory = unchecked((Int32) 0x8007000E),

        /// <summary>
        ///     E_NOINTERFACE
        /// </summary>
        NoInterface = unchecked((Int32) 0x80004002),

        /// <summary>
        ///     E_FAIL
        /// </summary>
        Fail = unchecked((Int32) 0x80004005),

        /// <summary>
        ///     E_ELEMENTNOTFOUND
        /// </summary>
        ElementNotFound = unchecked((Int32) 0x80070490),

        /// <summary>
        ///     TYPE_E_ELEMENTNOTFOUND
        /// </summary>
        TypeElementNotFound = unchecked((Int32) 0x8002802B),

        /// <summary>
        ///     NO_OBJECT
        /// </summary>
        NoObject = unchecked((Int32) 0x800401E5),

        /// <summary>
        ///     Win32 Error code: ERROR_CANCELLED
        /// </summary>
        Win32ErrorCanceled = 1223,

        /// <summary>
        ///     ERROR_CANCELLED
        /// </summary>
        Canceled = unchecked((Int32) 0x800704C7),

        /// <summary>
        ///     The requested resource is in use
        /// </summary>
        ResourceInUse = unchecked((Int32) 0x800700AA),

        /// <summary>
        ///     The requested resources is read-only.
        /// </summary>
        AccessDenied = unchecked((Int32) 0x80030005)
    }

    /// <summary>
    ///     Provide Error Message Helper Methods.
    ///     This is intended for Library Internal use only.
    /// </summary>
    internal static class CoreErrorHelper
    {
        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates failure.</returns>
        public static Boolean Failed(HResult result)
        {
            return !Succeeded(result);
        }

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates failure.</returns>
        public static Boolean Failed(Int32 result)
        {
            return !Succeeded(result);
        }

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        /// <param name="win32ErrorCode">The Windows API error code.</param>
        /// <returns>The equivalent HRESULT.</returns>
        public static Int32 HResultFromWin32(Int32 win32ErrorCode)
        {
            if (win32ErrorCode > 0)
                win32ErrorCode =
                    (Int32) (((UInt32) win32ErrorCode & 0x0000FFFF) | (FacilityWin32 << 16) | 0x80000000);
            return win32ErrorCode;
        }

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The COM error code.</param>
        /// <param name="win32ErrorCode">The Win32 error code.</param>
        /// <returns>Inticates that the Win32 error code corresponds to the COM error code.</returns>
        public static Boolean Matches(Int32 result, Int32 win32ErrorCode)
        {
            return result == HResultFromWin32(win32ErrorCode);
        }

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates success.</returns>
        public static Boolean Succeeded(Int32 result)
        {
            return result >= 0;
        }

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates success.</returns>
        public static Boolean Succeeded(HResult result)
        {
            return Succeeded((Int32) result);
        }

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        private const Int32 FacilityWin32 = 7;

        /// <summary>
        ///     This is intended for Library Internal use only.
        /// </summary>
        public const Int32 Ignored = (Int32) HResult.Ok;
    }
}