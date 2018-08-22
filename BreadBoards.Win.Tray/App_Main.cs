using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;
using Microsoft.Win32;

namespace BreadBoards.Win.Tray {
    public partial class App_Main : Form {
        internal DateTime dtAppStart { get; private set; } = DateTime.Now;
        internal DateTime? dtSysLocked { get; private set; }
        internal DateTime? dtSysUnLock { get; private set; }
        private string _TraceFileName;
        private string TraceFileName {
            get {
                if (string.IsNullOrEmpty(this._TraceFileName)) {
                    this._TraceFileName = Application.ExecutablePath.Substring(Application.ExecutablePath.LastIndexOf(@"\") + 1).Replace(".exe", $".TRACE.{DateTime.Now.ToString("yyyyMMdd")}.txt");
                }
                return this._TraceFileName;
            }
        }
        public App_Main() {
            this.InitializeComponent();
            // Setup Tracing
            // TraceFileName Is Unique Per Day
            using (var tw = new TextWriterTraceListener($@"C:\Windows\tracing\{this.TraceFileName}", "DefaultFile")) {
                Trace.Listeners.Add(tw);
            }
            Trace.AutoFlush = true;  // Save file after every Trace.Write
            Trace.WriteLine($"{this.dtAppStart.ToString("yyyy-MM-dd HH:mm:ss")} App Starting", "SessionLaunch");
            // Event Handling
            SystemEvents.SessionSwitch += this.SystemEvents_SessionSwitch;
            // Add Trace Listener To Update TextBox / Notification
            using (var tw = new Common.TextBoxTraceListener(this.txtConsoleLog, this.TrayIcon)) {
                Trace.Listeners.Add(tw);
            }
        }
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e) {
            switch (e.Reason) {
                case SessionSwitchReason.SessionLock:
                    this.dtSysLocked = DateTime.Now;
                    Trace.WriteLine($"{this.dtSysLocked.Value.ToString("yyyy-MM-dd HH:mm:ss")} {e.Reason.ToString()}", "SessionSwitch");
                    // Start the timer
                    this.tmrBreaks.Start();
                    break;
                case SessionSwitchReason.SessionUnlock:
                    this.dtSysUnLock = DateTime.Now;
                    var tsAway = (this.dtSysUnLock.Value - this.dtSysLocked.Value);
                    Trace.WriteLine($"{this.dtSysUnLock.Value.ToString("yyyy-MM-dd HH:mm:ss")} {e.Reason.ToString()}", "SessionSwitch");
                    Trace.WriteLineIf(this.dtSysLocked.HasValue, $"Time Away: {tsAway.ToString(@"hh\:mm\:ss")}");
                    this.TrayIcon.Visible = true;
                    this.TrayIcon.ShowBalloonTip(25000, "Welcome Back!!", $"Time Away: {tsAway.ToString(@"hh\:mm\:ss")}", ToolTipIcon.Info);
                    this.Visible = true;
                    this.Show();
                    this.BringToFront();
                    MessageBox.Show($"Welcome Back!\r\nYour total time away: {tsAway.ToString(@"hh\:mm\:ss")}", "Welcome Back!");
                    // Stop the timer
                    this.tmrBreaks.Stop();                    
                    break;
                default:
                    this.dtSysLocked = DateTime.Now;
                    Trace.WriteLine($"{this.dtSysLocked.Value.ToString("yyyy-MM-dd HH:mm:ss")} {e.Reason.ToString()}", "SessionSwitch");
                    break;
            }
        }
        private void TrayIcon_DoubleClick(object sender, EventArgs e) {
            switch (this.Visible) {
                case false:
                    this.ShowInTaskbar = true;
                    this.Visible = true;
                    this.Show();
                    break;
                case true:
                    this.ShowInTaskbar = false;
                    this.Visible = false;
                    this.Hide();
                    break;
            }
        }
        private void App_Main_Load(object sender, EventArgs e) {
            // Show TaskBar Icon
            this.TrayIcon.Visible = true;
            // Hide the form
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.Hide();
        }
        private void App_Main_FormClosing(object sender, FormClosingEventArgs e) {
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.Hide();
            // Don't Actually Let the application stop running 
            e.Cancel = true;
        }
    }
}
