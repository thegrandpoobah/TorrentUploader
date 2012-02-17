using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TorrentUploader.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public Int32 dwFlags;
        public UInt32 uCount;
        public Int32 dwTimeout;
    }

    [Flags]
    public enum FlashWindowExFlags : int
    {
        // stop flashing
        FLASHW_STOP = 0,

        // flash the window title 
        FLASHW_CAPTION = 1,

        // flash the taskbar button
        FLASHW_TRAY = 2,

        // 1 | 2
        FLASHW_ALL = 3,

        // flash continuously 
        FLASHW_TIMER = 4,

        // flash until the window comes to the foreground 
        FLASHW_TIMERNOFG = 12,
    }

    public static class Methods
    {
        [DllImport("user32.dll")]
        public static extern Int32 FlashWindowEx(ref FLASHWINFO pwfi);
    }

}
