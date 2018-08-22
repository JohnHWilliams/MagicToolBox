using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;

namespace BreadBoards.Win.Common {

    public class TextBoxTraceListener : TraceListener {
        private TextBox _target;
        private NotifyIcon _notify;
        private StringSendDelegate _invokeWrite;
        public TextBoxTraceListener(TextBox target, NotifyIcon notify) {
            this._target = target;
            this._notify = notify;
            this._invokeWrite = new StringSendDelegate(this.SendString);
        }
        public override void Write(string message) {
            this._target.Invoke(this._invokeWrite, new object[] { message });
        }
        public override void WriteLine(string message) {
            this._target.Invoke(this._invokeWrite, new object[] { message + Environment.NewLine });
        }
        private delegate void StringSendDelegate(string message);
        private void SendString(string message) {
            // No need to lock text box as this function will only 
            // ever be executed from the UI thread
            this._target.Text += message;
            this._notify.BalloonTipText = message;
            this._notify.ShowBalloonTip(5000);
        }
    }


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
