#define TRACE

using System;
using System.Windows.Forms;

namespace MagicToolBox.LunchTray {
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
