#define TRACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Diagnostics;
using evt=System.Diagnostics.Eventing;
using trc=System.Diagnostics.Tracing;

namespace BreadBoards.Win.Tray {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // Default Code
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new App_Main());
        }
    }
}
