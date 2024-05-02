// Copyright (c) Stéphane ANDRE. All Right Reserved.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace MyNet.Wpf.Helpers
{
    public static class NativeMethods
    {
        public static readonly IntPtr HwndTopMost = new(-1);
        public static readonly IntPtr HwndNoTopMost = new(-2);
        public static readonly IntPtr HwndTop = new(0);
        public static readonly IntPtr HwndBottom = new(1);

        /// <summary>
        /// SetWindowPos options
        /// </summary>
        [Flags]
        internal enum SWP
        {
            ASYNCWINDOWPOS = 0x4000,
            DEFERERASE = 0x2000,
            DRAWFRAME = 0x0020,
            HIDEWINDOW = 0x0080,
            NOACTIVATE = 0x0010,
            NOCOPYBITS = 0x0100,
            NOMOVE = 0x0002,
            NOOWNERZORDER = 0x0200,
            NOREDRAW = 0x0008,
            NOSENDCHANGING = 0x0400,
            NOSIZE = 0x0001,
            NOZORDER = 0x0004,
            SHOWWINDOW = 0x0040,
            TOPMOST = NOACTIVATE | NOOWNERZORDER | NOSIZE | NOMOVE | NOREDRAW | NOSENDCHANGING,
        }

        internal static int LOWORD(int i) => (short)(i & 0xFFFF);

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;

            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SIZE
        {
            public int cx;

            public int cy;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT : IEquatable<RECT>
        {
            public void Offset(int dx, int dy)
            {
                Left += dx;
                Top += dy;
                Right += dx;
                Bottom += dy;
            }

            public int Left { get; set; }

            public int Right { get; set; }

            public int Top { get; set; }

            public int Bottom { get; set; }

            public readonly int Width => Right - Left;

            public readonly int Height => Bottom - Top;

            public POINT Position => new() { x = Left, y = Top };

            public SIZE Size => new() { cx = Width, cy = Height };

            public static RECT Union(RECT rect1, RECT rect2) => new()
            {
                Left = Math.Min(rect1.Left, rect2.Left),
                Top = Math.Min(rect1.Top, rect2.Top),
                Right = Math.Max(rect1.Right, rect2.Right),
                Bottom = Math.Max(rect1.Bottom, rect2.Bottom),
            };

            public override readonly bool Equals(object? obj)
            {
                try
                {
                    var rc = (RECT?)obj;

                    return rc.HasValue
&& rc.Value.Bottom == Bottom
                           && rc.Value.Left == Left
                           && rc.Value.Right == Right
                           && rc.Value.Top == Top;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            public readonly bool Equals(RECT other) => throw new NotImplementedException();

            public override readonly int GetHashCode() => (Left << 16 | LOWORD(Right)) ^ (Top << 16 | LOWORD(Bottom));
        }

        [SecurityCritical]
        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRectInternal(IntPtr hWnd, out RECT lpRect);

        [SecurityCritical]
        internal static bool GetWindowRect(IntPtr hWnd, out RECT lpRect)
        {
            if (!GetWindowRectInternal(hWnd, out lpRect))
            {
                // If this fails it's never worth taking down the process.  Let the caller deal with the error if they want.
                return false;
            }

            return true;
        }

        [SecurityCritical]
        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPosInternal(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags);

        [SecurityCritical]
        internal static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SWP uFlags)
        {
            if (!SetWindowPosInternal(hWnd, hWndInsertAfter, x, y, cx, cy, uFlags))
            {
                // If this fails it's never worth taking down the process.  Let the caller deal with the error if they want.
                return false;
            }

            return true;
        }

        [Flags]
        public enum ExtendedWindowStyles
        {
            WS_EX_TOOLWINDOW = 0x00000080,
        }

        public enum GetWindowLongFields
        {
            GWL_EXSTYLE = (-20),
        }

        public static IntPtr GetWindowLongWrapper(IntPtr hWnd, int nIndex)
        {
            if (nIndex > int.MinValue)
                return GetWindowLong(hWnd, nIndex);

            var result = hWnd;
            return result;
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        private static extern void SetLastError(int dwErrorCode);

        public static void SetLastErrorWrapper(int dwErrorCode)
        {
            if (dwErrorCode > int.MinValue)
                SetLastError(dwErrorCode);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            // Win32 SetWindowLong doesn't clear error on success
            SetLastErrorWrapper(0);

            int error;
            IntPtr result;
            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                var tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            return (result == IntPtr.Zero) && (error != 0) ? throw new System.ComponentModel.Win32Exception(error) : result;
        }

        private static int IntPtrToInt32(IntPtr intPtr) => unchecked((int)intPtr.ToInt64());

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int IntSetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }
}
