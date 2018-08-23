using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace BreadBoards.Win.Common {
    public class TrayApplicationContext : ApplicationContext {
        private NotifyIcon trayIcon;

        public TrayApplicationContext() {
            // Initialize Tray Icon
            this.trayIcon = new NotifyIcon() {
                Icon = Icon.FromHandle(Tray.Properties.Resources.ClockOut32.GetHicon()),
                ContextMenu = new ContextMenu(
                    new MenuItem[] {
                        new MenuItem("Exit", this.Exit)
                       ,new MenuItem()
                    }
                ),
                Visible = true,                
            };
        }
        public void Exit(object sender, EventArgs e) {
            // Hide tray icon, otherwise it will remain shown until user mouses over it
            this.trayIcon.Visible = false;
            Application.Exit();
        }
    }


}
