using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Das.Views.Core.Geometry;
using Das.Views.Core.Input;
using Das.Views.Input;
using Java.Interop;

namespace Das.Xamarin.Android
{
    public class InputContext : IInputContext,
                                GestureDetector.IOnGestureListener,
                                GestureDetector.IOnDoubleTapListener,
                                View.IOnTouchListener
    {
        public IPoint2D CursorPosition { get; }

        public Boolean IsCapsLockOn { get; }

        public Boolean AreButtonsPressed(KeyboardButtons button1, 
                                         KeyboardButtons button2)
        {
            throw new NotImplementedException();
        }

        public Boolean AreButtonsPressed(KeyboardButtons button1, 
                                         KeyboardButtons button2, 
                                         KeyboardButtons button3)
        {
            throw new NotImplementedException();
        }

        public Boolean AreButtonsPressed(MouseButtons button1, MouseButtons button2)
        {
            throw new NotImplementedException();
        }

        public Boolean AreButtonsPressed(MouseButtons button1, MouseButtons button2, MouseButtons button3)
        {
            throw new NotImplementedException();
        }

        public Boolean IsButtonPressed(KeyboardButtons keyboardButton)
        {
            throw new NotImplementedException();
        }

        public Boolean IsButtonPressed(MouseButtons mouseButton)
        {
            throw new NotImplementedException();
        }

        public Boolean IsMousePresent { get; }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IntPtr Handle { get; }

        public void SetJniIdentityHashCode(Int32 value)
        {
            throw new NotImplementedException();
        }

        public void SetPeerReference(JniObjectReference reference)
        {
            throw new NotImplementedException();
        }

        public void SetJniManagedPeerState(JniManagedPeerStates value)
        {
            throw new NotImplementedException();
        }

        public void UnregisterFromRuntime()
        {
            throw new NotImplementedException();
        }

        public void DisposeUnlessReferenced()
        {
            throw new NotImplementedException();
        }

        public void Disposed()
        {
            throw new NotImplementedException();
        }

        public void Finalized()
        {
            throw new NotImplementedException();
        }

        public Int32 JniIdentityHashCode { get; }

        public JniObjectReference PeerReference { get; }

        public JniPeerMembers JniPeerMembers { get; }

        public JniManagedPeerStates JniManagedPeerState { get; }

        public Boolean OnTouch(View? v, MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnDown(MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnFling(MotionEvent? e1, MotionEvent? e2, Single velocityX, Single velocityY)
        {
            throw new NotImplementedException();
        }

        public void OnLongPress(MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnScroll(MotionEvent? e1, MotionEvent? e2, Single distanceX, Single distanceY)
        {
            throw new NotImplementedException();
        }

        public void OnShowPress(MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnSingleTapUp(MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnDoubleTap(MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnDoubleTapEvent(MotionEvent? e)
        {
            throw new NotImplementedException();
        }

        public Boolean OnSingleTapConfirmed(MotionEvent? e)
        {
            throw new NotImplementedException();
        }
    }
}