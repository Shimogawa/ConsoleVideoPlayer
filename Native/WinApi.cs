using System;
using System.Runtime.InteropServices;

namespace ConsoleVideoPlayer.Native
{
    public class WinApi
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int    X,
            int    Y,
            int    cx,
            int    cy,
            int    uFlags);

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
            string lpFileName,
            int    dwDesiredAccess,
            int    dwShareMode,
            IntPtr lpSecurityAttributes,
            int    dwCreationDisposition,
            int    dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool   bMaximumWindow,
            [Out] [MarshalAs(UnmanagedType.LPStruct)]
            ConsoleFontInfo lpConsoleCurrentFont);

        [StructLayout(LayoutKind.Sequential)]
        public class ConsoleFontInfo
        {
            public int   nFont;
            public Coord dwFontSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Coord
        {
            [FieldOffset(0)] public short X;
            [FieldOffset(2)] public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner
        }

        public const int GENERIC_READ         = unchecked((int) 0x80000000);
        public const int GENERIC_WRITE        = 0x40000000;
        public const int FILE_SHARE_READ      = 1;
        public const int FILE_SHARE_WRITE     = 2;
        public const int INVALID_HANDLE_VALUE = -1;
        public const int OPEN_EXISTING        = 3;

        public const int MF_BYCOMMAND         = 0x00000000;

        public const int SC_CLOSE             = 0xF060;
        public const int SC_MINIMIZE          = 0xF020;
        public const int SC_MAXIMIZE          = 0xF030;
        public const int SC_SIZE              = 0xF000;

        public const int SWP_NOSIZE           = 0x0001;
        public const int SWP_NOZORDER         = 0x0004;
        public const int SWP_SHOWWINDOW       = 0x0040;
    }
}