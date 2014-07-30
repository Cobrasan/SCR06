using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Alchemist
{
    public class WinAPI
    {
        public const int WM_CLOSE = 0x10;

        [DllImport("user32", EntryPoint = "SendMessageA")]
        public static extern Int32 SendMessage(Int32 hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
    }
}
