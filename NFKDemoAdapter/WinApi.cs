using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NFKDemoAdapter
{
    public class WinApi
    {
        int hWnd = 0;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int SW_FORCEMINIMIZE = 6;

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const int VK_TAB = 0x09;


        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        public static void ShowWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_SHOW);
            }
        }

        public static void HideWindow(IntPtr hWnd)
        {
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, SW_HIDE);
            }
        }


        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);


        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr point);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr window, out int process);

        public static IntPtr[] GetProcessWindows(int pid)
        {
            IntPtr[] apRet = (new IntPtr[256]);
            int iCount = 0;
            IntPtr pLast = IntPtr.Zero;
            do
            {
                pLast = FindWindowEx(IntPtr.Zero, pLast, null, null);
                int iProcess_;
                GetWindowThreadProcessId(pLast, out iProcess_);
                if (iProcess_ == pid)
                    apRet[iCount++] = pLast;
            } while (pLast != IntPtr.Zero);
            System.Array.Resize(ref apRet, iCount);
            return apRet;
        }



        public static class InterceptKeys
        {
            private const int WH_KEYBOARD_LL = 13;
            private const int WM_ESC = 0x1B;
            private const int WM_KEYDOWN = 0x0100;
            private static LowLevelKeyboardProc _proc = HookCallback;
            private static IntPtr _hookID = IntPtr.Zero;

            public static void HookEsc(IntPtr hWnd)
            {
                _hookID = SetHook(_proc, hWnd);
            }
            public static void UnhookEsc()
            {
                UnhookWindowsHookEx(_hookID);
            }
            private static IntPtr SetHook(LowLevelKeyboardProc proc, IntPtr hWnd)
            {
                //using (Process curProcess = Process.GetCurrentProcess())
                //using (ProcessModule curModule = curProcess.MainModule)
                //{
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, hWnd, 0);
                //}
            }

            private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

            /// <summary>
            /// Kill process by esc
            /// </summary>
            /// <param name="nCode"></param>
            /// <param name="wParam"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            private static IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
            {
                if (lParam.vkCode == WM_ESC && wParam == (IntPtr)WM_KEYDOWN)
                {
                    int pid;
                    WinApi.GetWindowThreadProcessId(wParam, out pid);
                    var p = Process.GetProcessById(pid);
                    p.Kill();
                }

                return CallNextHookEx(_hookID, nCode, wParam, ref lParam);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();


            private struct KBDLLHOOKSTRUCT
            {
                public int vkCode;
                int scanCode;
                public int flags;
                int time;
                int dwExtraInfo;
            }
        }
    }
}
