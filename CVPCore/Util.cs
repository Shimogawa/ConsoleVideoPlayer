using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using static ConsoleVideoPlayer.Native.WinApi;

namespace ConsoleVideoPlayer.CVPCore
{
    public class Util
    {
        public static Size GetConsoleFontSize()
        {
            // getting the console out buffer handle
            IntPtr outHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            int errorCode = Marshal.GetLastWin32Error();
            if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", errorCode);
            }

            ConsoleFontInfo cfi = new ConsoleFontInfo();
            if (!GetCurrentConsoleFont(outHandle, false, cfi))
            {
                throw new InvalidOperationException("Unable to get font information.");
            }

            return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
        }

        public static void FixConsoleWindowSize()
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);

            if (handle == IntPtr.Zero)
            {
                throw new SystemException("Cannot get console window handle.");
            }

            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
        }

        public static void AdjustConsoleWindowSize(Size size)
        {
            IntPtr handle = GetConsoleWindow();
            if (handle != IntPtr.Zero)
            {
                RECT rct;
                GetWindowRect(handle, out rct);

                SetWindowPos(handle, IntPtr.Zero, rct.Left, rct.Top, size.Width, size.Height,
                    SWP_NOZORDER | SWP_SHOWWINDOW);
            }
        }
    }
}