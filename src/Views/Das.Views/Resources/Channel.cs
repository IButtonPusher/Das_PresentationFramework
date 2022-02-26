using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Das.Views.Validation;

namespace Das.Views.Resources
{
    /// <summary>
        /// A Channel is a command pipe into a composition device.
        /// The commands send through a Channel are not executed till
        /// Channel.Commit is called. Committing a Channel is an atomic operation. In
        /// other words, all the commands are executed before the next frame is
        /// rendered.
        ///
        /// A channel is also a hard boundary for UCE resources. That means that UCE
        /// resources created on one channel can not interact with resources on a different
        /// channel.
        /// </summary>
        public sealed partial class Channel
        {
            /// <summary>
            /// Primary channel.
            /// </summary>
            IntPtr _hChannel;

            private Channel _referenceChannel;
            private bool _isSynchronous;
            private bool _isOutOfBandChannel;

            IntPtr _pConnection;

            /// <summary>
            /// Creates a channel and associates it with channel group (partition).
            /// New create channel will belong to the same partition as the given referenceChannel.
            /// To create the very first channel in the group, use null argument.
            /// </summary>
            public Channel(Channel referenceChannel, bool isOutOfBandChannel, IntPtr pConnection, bool isSynchronous)
            {
                IntPtr referenceChannelHandle = IntPtr.Zero;

                _referenceChannel = referenceChannel;
                _pConnection = pConnection;
                _isOutOfBandChannel = isOutOfBandChannel;
                _isSynchronous = isSynchronous;

                if (referenceChannel != null)
                {
                    referenceChannelHandle = referenceChannel._hChannel;
                }

                HRESULT.Check(UnsafeNativeMethods.MilConnection_CreateChannel(
                    _pConnection,
                    referenceChannelHandle,
                    out _hChannel));
}


            /// <summary>
            /// Commits the commands enqueued into the Channel.
            /// </summary>
            public void Commit()
            {
                if (_hChannel == IntPtr.Zero)
                {
                    //
                    // If the channel has been closed, fail silently. This could happen
                    // for the service channel if we are in disconnected state when more
                    // that one media contexts are present and not all of them have finished
                    // processing the disconnect messages.
                    //

                    return;
                }

                HRESULT.Check(UnsafeNativeMethods.MilConnection_CommitChannel(
                   _hChannel));
            }

            /// <summary>
            /// Closes the current batch on the Channel.
            /// </summary>
            public void CloseBatch()
            {
                if (_hChannel == IntPtr.Zero)
                {
                    //
                    // If the channel has been closed, fail silently. This could happen
                    // for the service channel if we are in disconnected state when more
                    // that one media contexts are present and not all of them have finished
                    // processing the disconnect messages.
                    //

                    return;
                }

                HRESULT.Check(UnsafeNativeMethods.MilConnection_CloseBatch(
                   _hChannel));
            }

            /// <summary>
            ///   Flush the currently recorded commands to the target device and prepare
            ///   to receive new commands. Block until last command was executed.
            /// </summary>
            public void SyncFlush()
            {
                if (_hChannel == IntPtr.Zero)
                {
                    //
                    // If the channel has been closed, fail silently. This could happen
                    // for the service channel if we are in disconnected state whhen more
                    // that one media contexts are present and not all of them have finished
                    // processing the disconnect messages.
                    //

                    return;
                }

                HRESULT.Check(MilCoreApi.MilComposition_SyncFlush(_hChannel));
}

            /// <summary>
            /// Commits the channel and then closes it.
            /// </summary>
            public void Close()
            {
                if (_hChannel != IntPtr.Zero)
                {
                    HRESULT.Check(UnsafeNativeMethods.MilConnection_CloseBatch(_hChannel));
                    HRESULT.Check(UnsafeNativeMethods.MilConnection_CommitChannel(_hChannel));
                }

                _referenceChannel = null;

                if (_hChannel != IntPtr.Zero)
                {
                    HRESULT.Check(UnsafeNativeMethods.MilConnection_DestroyChannel(_hChannel));

                    _hChannel = IntPtr.Zero;
                }
            }

            /// <summary>
            /// Commits the commands enqueued into the Channel.
            /// </summary>
            public void Present()
            {
                HRESULT.Check(UnsafeNativeMethods.WgxConnection_SameThreadPresent(_pConnection));
            }

            /// <summary>
            /// public only: CreateOrAddRefOnChannel addrefs the resource corresponding to the
            /// specified handle on the channel.
            /// </summary>
            /// <return>
            /// Returns true iff the resource was created on the channel. The caller is responsible to
            /// update the resource appropriately.
            /// </return>
            public bool CreateOrAddRefOnChannel(object instance, ref ResourceHandle handle, ResourceType resourceType)
            {
                bool handleNeedsCreation = handle.IsNull;

                Invariant.Assert(_hChannel != IntPtr.Zero);

                HRESULT.Check(UnsafeNativeMethods.MilResource_CreateOrAddRefOnChannel(
                    _hChannel,
                    resourceType,
                    ref handle
                    ));

                if (EventTrace.IsEnabled(EventTrace.Keyword.KeywordGraphics | EventTrace.Keyword.KeywordPerf, EventTrace.Level.PERF_LOW))
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.Event.CreateOrAddResourceOnChannel, EventTrace.Keyword.KeywordGraphics | EventTrace.Keyword.KeywordPerf, EventTrace.Level.PERF_LOW, PerfService.GetPerfElementID(instance), _hChannel, (uint) handle, (uint) resourceType); 
                }

                return handleNeedsCreation;
            }

            /// <summary>
            /// DuplicateHandle attempts to duplicate a handle from one channel to another.
            /// Naturally, this can only work if both the source and target channels are
            /// within the same partition.
            /// </summary>
            /// <remarks>
            /// It is the responsibility of the caller to commit the source channel
            /// to assure that duplication took place.
            /// tables.
            /// </remarks>
            /// <return>
            /// Returns the duplicated handle (valid on the target channel) or the null
            /// handle if duplication failed.
            /// </return>
            public ResourceHandle DuplicateHandle(
                ResourceHandle original,
                Channel targetChannel
                )
            {
                ResourceHandle duplicate = ResourceHandle.Null;

                //Debug.WriteLine(string.Format("DuplicateHandle: Channel: {0}, Resource: {1}, Target channel: {2},  ", _hChannel, original._handle, targetChannel));

                HRESULT.Check(UnsafeNativeMethods.MilResource_DuplicateHandle(
                    _hChannel,
                    original,
                    targetChannel._hChannel,
                    ref duplicate
                    ));

                return duplicate;
            }


            /// <summary>
            /// public only: ReleaseOnChannel releases the resource corresponding to the specified
            /// handle on the channel.
            /// </summary>
            /// <return>
            /// Returns true iff the resource is not on this channel anymore.
            /// </return>
            public bool ReleaseOnChannel(ResourceHandle handle)
            {
                Invariant.Assert(_hChannel != IntPtr.Zero);
                Debug.Assert(!handle.IsNull);

                //Debug.WriteLine(string.Format("ReleaseOnChannel: Channel: {0}, Resource: {1}", _hChannel, handle._handle));

                int releasedOnChannel;

                HRESULT.Check(UnsafeNativeMethods.MilResource_ReleaseOnChannel(
                    _hChannel,
                    handle,
                    out releasedOnChannel
                    ));

                if ((releasedOnChannel != 0) && EventTrace.IsEnabled(EventTrace.Keyword.KeywordGraphics | EventTrace.Keyword.KeywordPerf, EventTrace.Level.PERF_LOW))
                {
                    EventTrace.EventProvider.TraceEvent(EventTrace.Event.ReleaseOnChannel, EventTrace.Keyword.KeywordGraphics | EventTrace.Keyword.KeywordPerf, EventTrace.Level.PERF_LOW, _hChannel, (uint) handle); 
                }

                return (releasedOnChannel != 0);
            }

            /// <summary>
            /// public only: GetRefCount returns the reference count of a resource 
            /// corresponding to the specified handle on the channel.
            /// </summary>
            /// <return>
            /// Returns the ref count for a resource on this channel.
            /// </return>
            public uint GetRefCount(ResourceHandle handle)
            {
                Invariant.Assert(_hChannel != IntPtr.Zero);
                Debug.Assert(!handle.IsNull);

                uint refCount;

                HRESULT.Check(UnsafeNativeMethods.MilResource_GetRefCountOnChannel(
                    _hChannel,
                    handle,
                    out refCount
                    ));

                return refCount;
            }


            /// <summary>
            /// IsConnected returns true if the channel is connected.
            /// </summary>
            public bool IsConnected
            {
                get
                {
                    return MediaContext.CurrentMediaContext.IsConnected;
                }
            }

            /// <summary>
            /// MarshalType returns the marshal type of the channel.
            /// </summary>
            public ChannelMarshalType MarshalType
            {
                get
                {
                    Invariant.Assert(_hChannel != IntPtr.Zero);

                    ChannelMarshalType marshalType;
                    HRESULT.Check(UnsafeNativeMethods.MilChannel_GetMarshalType(
                        _hChannel,
                        out marshalType
                        ));

                    return marshalType;
                }
            }

            /// <summary>
            /// Returns whether the given channel is synchronous.
            /// </summary>
            public bool IsSynchronous
            {
                get
                {
                    return _isSynchronous;
                }
            }

            /// <summary>
            /// Returns whether the given channel is an out of band channel.
            /// </summary>
            public bool IsOutOfBandChannel
            {
                get
                {
                    return _isOutOfBandChannel;
                }
            }

            /// <summary>
            /// SendCommand sends a command struct through the composition thread.
            /// </summary>
            unsafe public void SendCommand(
                byte *pCommandData,
                int cSize)
            {
                SendCommand(pCommandData, cSize, false);
            }

            /// <summary>
            /// SendCommand sends a command struct through the composition thread. The
            /// sendInSeparateBatch parameter determines whether the command is sent in the
            /// current open batch, or whether it will be added to a new and separate batch
            /// which is then immediately closed, leaving the current batch untouched.
            /// </summary>
            unsafe public void SendCommand(
                byte *pCommandData,
                int cSize,
                bool sendInSeparateBatch)
            {
                checked
                {
                    Invariant.Assert(pCommandData != (byte*)0 && cSize > 0);

                    int hr = HRESULT.S_OK;

                    if (_hChannel == IntPtr.Zero)
                    {
                        //
                        // If the channel has been closed, fail silently. This could happen
                        // for the service channel if we are in disconnected state when more
                        // that one media contexts are present and not all of them have finished
                        // processing the disconnect messages.
                        //

                        return;
                    }

                    hr = UnsafeNativeMethods.MilResource_SendCommand(
                        pCommandData,
                        (uint)cSize,
                        sendInSeparateBatch,
                        _hChannel);

                    HRESULT.Check(hr);
                }
            }

            /// <summary>
            /// BeginCommand opens a command on a channel
            /// </summary>
            unsafe public void BeginCommand(
                byte *pbCommandData,
                int cbSize,
                int cbExtra)
            {
                checked
                {
                    Invariant.Assert(cbSize > 0);

                    int hr = HRESULT.S_OK;

                    if (_hChannel == IntPtr.Zero)
                    {
                        //
                        // If the channel has been closed, fail silently. This could happen
                        // for the service channel if we are in disconnected state whhen more
                        // that one media contexts are present and not all of them have finished
                        // processing the disconnect messages.
                        //

                        return;
                    }

                    hr = UnsafeNativeMethods.MilChannel_BeginCommand(
                        _hChannel,
                        pbCommandData,
                        (uint)cbSize,
                        (uint)cbExtra
                        );

                    HRESULT.Check(hr);
                }
            }

            /// <summary>
            /// AppendCommandData appends data to an open command on a channel
            /// </summary>
            unsafe public void AppendCommandData(
                byte *pbCommandData,
                int cbSize)
            {
                checked
                {
                    Invariant.Assert(pbCommandData != (byte*)0 && cbSize > 0);

                    int hr = HRESULT.S_OK;

                    if (_hChannel == IntPtr.Zero)
                    {
                        //
                        // If the channel has been closed, fail silently. This could happen
                        // for the service channel if we are in disconnected state whhen more
                        // that one media contexts are present and not all of them have finished
                        // processing the disconnect messages.
                        //

                        return;
                    }

                    hr = UnsafeNativeMethods.MilChannel_AppendCommandData(
                        _hChannel,
                        pbCommandData,
                        (uint)cbSize
                        );

                    HRESULT.Check(hr);
                }
            }

            /// <summary>
            /// EndCommand closes an open command on a channel
            /// </summary>
            public void EndCommand()
            {
                if (_hChannel == IntPtr.Zero)
                {
                    //
                    // If the channel has been closed, fail silently. This could happen
                    // for the service channel if we are in disconnected state whhen more
                    // that one media contexts are present and not all of them have finished
                    // processing the disconnect messages.
                    //

                    return;
                }

                HRESULT.Check(UnsafeNativeMethods.MilChannel_EndCommand(_hChannel));
            }

            /// <summary>
            /// SendCommand that creates an slave bitmap resource
            /// </summary>
            public void SendCommandBitmapSource(
                ResourceHandle imageHandle,
                BitmapSourceSafeMILHandle pBitmapSource
                )
            {
                Invariant.Assert(pBitmapSource != null && !pBitmapSource.IsInvalid);
                Invariant.Assert(_hChannel != IntPtr.Zero);

                HRESULT.Check(UnsafeNativeMethods.MilResource_SendCommandBitmapSource(
                    imageHandle,
                    pBitmapSource,
                    _hChannel));
            }

            /// <summary>
            /// SendCommand that creates an slave media resource
            /// </summary>
            public void SendCommandMedia(
                ResourceHandle mediaHandle,
                SafeMediaHandle pMedia,
                bool notifyUceDirect
                )
            {
                Invariant.Assert(pMedia != null && !pMedia.IsInvalid);

                Invariant.Assert(_hChannel != IntPtr.Zero);

                HRESULT.Check(UnsafeNativeMethods.MilResource_SendCommandMedia(
                    mediaHandle,
                    pMedia,
                    _hChannel,
                    notifyUceDirect
                    ));
            }

            /// <summary>
            /// Specifies the window and window message to be sent when messages
            /// become available in the back channel.
            /// </summary>
            /// <param name="hwnd">
            /// The target of the notification messages. If this parameter is null
            /// then the channel stop sending window messages.
            /// </param>
            /// <param name="message">
            /// The window message ID. If the hwnd parameter is null then this
            /// parameter is ignored.
            /// </param>
            public void SetNotificationWindow(IntPtr hwnd, WindowMessage message)
            {
                Invariant.Assert(_hChannel != IntPtr.Zero);

                HRESULT.Check(UnsafeNativeMethods.MilChannel_SetNotificationWindow(
                    _hChannel,
                    hwnd,
                    message
                    ));
            }

            /// <summary>
            /// Waits until a message is available on this channel. The message
            /// can be later retrieved with the PeekNextMessage method.
            /// </summary>
            /// <remarks>
            /// The method may return with no available messages if the channel
            /// is disconnected while waiting.
            /// </remarks>
            public void WaitForNextMessage()
            {
                int waitReturn;

                HRESULT.Check(UnsafeNativeMethods.MilComposition_WaitForNextMessage(
                    _hChannel,
                    0,
                    null,
                    1, /* true */
                    waitInfinite,
                    out waitReturn
                    ));
            }

            /// <summary>
            /// Gets the next available message on this channel. This method
            /// does not wait if a message is not immediately available.
            /// </summary>
            /// <param name="message">
            /// Receives the message.
            /// </param>
            /// <returns>
            /// True if a message was retrieved, false otherwise.
            /// </returns>
            public bool PeekNextMessage(out MilMessage.Message message)
            {
                Invariant.Assert(_hChannel != IntPtr.Zero);

                int messageRetrieved;

                checked
                {
                    unsafe
                    {
                        HRESULT.Check(UnsafeNativeMethods.MilComposition_PeekNextMessage(
                            _hChannel,
                            out message,
                            (IntPtr)sizeof(MilMessage.Message),
                            out messageRetrieved
                            ));
                    }
                }

                return (messageRetrieved != 0);
            }
        }
}
