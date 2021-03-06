// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable
using Microsoft.Win32.SafeHandles;

namespace System.Net
{
#if DEBUG
    //
    // This is a helper class for debugging GC-ed handles that we define.
    // As a general rule normal code path should always destroy handles explicitly
    //
    internal abstract class DebugCriticalHandleMinusOneIsInvalid : CriticalHandleMinusOneIsInvalid
    {
        private string _trace;

        protected DebugCriticalHandleMinusOneIsInvalid() : base()
        {
            _trace = "WARNING! GC-ed  >>" + this.GetType().FullName + "<< (should be explicitly closed) \r\n";
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, "Creating SafeHandle");
        }

        ~DebugCriticalHandleMinusOneIsInvalid()
        {
            if (NetEventSource.IsEnabled) NetEventSource.Info(this, _trace);
        }
    }
#endif // DEBUG
}
