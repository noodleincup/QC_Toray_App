using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dummy_project
{
    public static class ConsoleHelper
    {
        // Allocates a new console for the calling process.
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool AllocConsole();

        // Detaches the calling process from its console.
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeConsole();

        // Retrieves the window handle of the console window.
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        // Shows or hides the specified window.
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        // Constants for ShowWindow
        internal const int SW_HIDE = 0;
        internal const int SW_SHOW = 5;
    }
}
