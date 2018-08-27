using System;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using cfg = System.Configuration.ConfigurationManager;

namespace MagicToolBox.LunchTray {
    public partial class App_Main : Form {

        #region " Types "
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
        #endregion

        #region " Properties "
            internal DateTime AppStart { get; private set; } = DateTime.Now;
            internal DateTime? SysLocked { get; private set; }
            internal DateTime? SysUnLock { get; private set; }
            private string _TraceFileName;
            private string TraceFileName {
                get {
                    if (string.IsNullOrEmpty(this._TraceFileName)) {
                        // TraceFileName Is Unique Per Day
                        this._TraceFileName = string.Format(cfg.AppSettings["TraceFileNameFormat"], DateTime.Now);
                    }
                    return this._TraceFileName;
                }
            }
        #endregion

        #region " Constructors "
            public App_Main() {
                this.InitializeComponent();
                // Setup Tracing            
                using (var tw = new TextWriterTraceListener($@"{cfg.AppSettings["TracePath"]}{this.TraceFileName}", "DefaultFile")) {
                    Trace.Listeners.Add(tw);
                }
                Trace.AutoFlush = true;  // Save file after every Trace.Write
                Trace.WriteLine($"{this.AppStart.ToString("yyyy-MM-dd HH:mm:ss")} App Starting", "SessionLaunch");
                // Event Handling
                SystemEvents.SessionSwitch += this.SystemEvents_SessionSwitch;
                // Add Trace Listener To Update TextBox / Notification
                using (var tw = new TextBoxTraceListener(this.txtConsoleLog, this.TrayIcon)) {
                    Trace.Listeners.Add(tw);
                }
            }
        #endregion

        #region " Events "
            private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e) {
                switch (e.Reason) {
                    case SessionSwitchReason.SessionLock:
                        this.SysLocked = DateTime.Now;
                        Trace.WriteLine($"{this.SysLocked.Value.ToString("yyyy-MM-dd HH:mm:ss")} {e.Reason.ToString()}", "SessionSwitch");
                        // Start the timer
                        this.tmrBreaks.Start();
                        break;
                    case SessionSwitchReason.SessionUnlock:
                        this.SysUnLock = DateTime.Now;
                        var tsAway = (this.SysUnLock.Value - this.SysLocked.Value);
                        Trace.WriteLine($"{this.SysUnLock.Value.ToString("yyyy-MM-dd HH:mm:ss")} {e.Reason.ToString()}", "SessionSwitch");
                        Trace.WriteLineIf(this.SysLocked.HasValue, $"Time Away: {tsAway.ToString(@"hh\:mm\:ss")}");
                        this.Visible = true;
                        this.Show();
                        this.Activate();
                        MessageBox.Show($"Welcome Back!\r\nYour total time away: {tsAway.ToString(@"hh\:mm\:ss")}", "Welcome Back!");
                        this.TrayIcon.Visible = true;
                        this.TrayIcon.ShowBalloonTip(25000, "Welcome Back!!", $"Time Away: {tsAway.ToString(@"hh\:mm\:ss")}", ToolTipIcon.Info);
                        Application.DoEvents();
                        // Stop the timer
                        this.tmrBreaks.Stop();                    
                        break;
                    default:
                        this.SysLocked = DateTime.Now;
                        Trace.WriteLine($"{this.SysLocked.Value.ToString("yyyy-MM-dd HH:mm:ss")} {e.Reason.ToString()}", "SessionSwitch");
                        break;
                }
            }
            private void TrayIcon_DoubleClick(object sender, EventArgs e) {
                switch (this.Visible) {
                    case false:
                        this.ShowInTaskbar = true;
                        this.Visible = true;
                        this.Show();
                        this.Activate();
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
        #endregion

        #region " Methods "

        #endregion
        
    }
}
