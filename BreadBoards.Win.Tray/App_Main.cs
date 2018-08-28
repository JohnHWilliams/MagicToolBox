using Microsoft.Win32;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
// Custom Alias Usings
using cfg = System.Configuration.ConfigurationManager;
using Magic = MagicToolBox.Shared.Win;
using System.Windows.Controls.Primitives;

namespace MagicToolBox.LunchTray {
    public partial class App_Main : Form {

        #region " Types "
            private class SessionEvent {
                public int ID { set; get; }
                public SessionSwitchReason EventTypeID { set; get; }
                public DateTime Start { set; get; }
                public DateTime Ended { set; get; }
                public string Message { set; get; }
            }
        #endregion

        #region " Properties "
            internal DateTime AppStart { get; private set; }
            internal DateTime? SysLocked { get; private set; }
            internal DateTime? SysUnLock { get; private set; }
            internal DateTime? SysLogOn { get; private set; }
            internal DateTime? SysLogOff { get; private set; }
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
            private SessionEvent ActiveEvent { set; get; }
            private TimeSpan tsWorking { set; get; }
            private TimeSpan tsOnBreak { set; get; }
        #endregion

        #region " Constructors "
            public App_Main() {
                this.InitializeComponent();
                // Initialize Property Values
                this.AppStart = DateTime.Now;
                this.SysUnLock = DateTime.Now;  // Set this to Now to keep the logic simple later
                // Add Event Handler for when the workstation 
                SystemEvents.SessionSwitch += this.SystemEvents_SessionSwitch;
                // Create a partial event to indicate we're beginning a period where the workstation is unlocked (Actively Working/At Workstation/NOT on break)
                this.ActiveEvent = new SessionEvent() { Start = DateTime.Now, EventTypeID = SessionSwitchReason.SessionUnlock };
                // Start Working Timer
                this.tmrWork.Start();
            }
        #endregion

        #region " Events "
            private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e) {
                switch (e.Reason) {
                    case SessionSwitchReason.SessionLock:
                    case SessionSwitchReason.SessionLogoff:
                        // Start the timer
                        this.tmrBreak.Start();  // INFO: This behaves like a javascript interval NOT like a stopwatch  TODO: Remove ??

                        // Update relative DateTime properties
                        this.SysLocked = DateTime.Now;
                        if(e.Reason == SessionSwitchReason.SessionLogoff) this.SysLogOff = DateTime.Now;
                        var tsWork = (this.SysLocked.Value - this.SysUnLock.Value); // Initialized UnLock DateTime Value on construct so it's safe to use this and not worry about logic to figure it out

                        // We're ending a period where the workstation was unlocked ( NOT on break )
                        this.ActiveEvent.EventTypeID = SessionSwitchReason.SessionUnlock;
                        this.ActiveEvent.Ended = DateTime.Now;
                        this.ActiveEvent.Message = $"Enjoy Break! Time Worked: {tsWork.ToString(@"dd\:hh\:mm\:ss")} SwitchReason: {e.Reason.ToString()}";

                        // Save the Event to the database
                        this.SessionEventLog_Insert(this.ActiveEvent);

                        // Trace the event to the text file
                        Trace.WriteLine(this.ActiveEvent.Message);

                        // TODO: Try showing/activating the form on locking so it will already be on top when you unlock the workstation 
                        this.Visible = true;
                        this.Show();
                        this.Activate();
                        Application.DoEvents();

                        // Start the next event
                        this.ActiveEvent = new SessionEvent() { Start = this.SysLocked.Value, EventTypeID = e.Reason };

                        break;
                    case SessionSwitchReason.SessionUnlock:
                    case SessionSwitchReason.SessionLogon:  // TODO: Capturing LogON UNLIKELY as the app will only be able to start AFTER logging on soooo..... (??)
                        // Stop the timer
                        this.tmrBreak.Stop();

                        // Update relative DateTime properties
                        this.SysUnLock = DateTime.Now;
                        var tsAway = (this.SysUnLock.Value - this.SysLocked.Value);

                        // We're ending a period where the workstation was locked ( WAS ON break )
                        this.ActiveEvent.EventTypeID = SessionSwitchReason.SessionLock;
                        this.ActiveEvent.Ended = DateTime.Now;
                        this.ActiveEvent.Message = $"Welcome Back!  Time Away: {tsAway.ToString(@"dd\:hh\:mm\:ss")} - {e.Reason.ToString()}";

                        // Save the Event to the database
                        this.SessionEventLog_Insert(this.ActiveEvent);
                        Trace.WriteLine(this.ActiveEvent.Message);

                        // Show the form to welcome the user back
                        this.Visible = true;
                        this.Show();
                        this.Activate();

                        // Setup the TrayIcon to notify the user
                        this.TrayIcon.Visible = true;
                        this.TrayIcon.Text = $"Time Away: {tsAway.ToString(@"dd\:hh\:mm\:ss")}";
                        this.TrayIcon.BalloonTipText = this.TrayIcon.Text;
                        this.TrayIcon.ShowBalloonTip(25000, "Welcome Back!!", this.TrayIcon.Text, ToolTipIcon.Info);                        

                        // TODO: See if this will make the TrayIcon.ShowBallonTip work (BECAUSE IT JUST WON'T F*#@@$ SHOW GRRRRR!!!)
                        Application.DoEvents();

                        // Show a Message Box
                        MessageBox.Show($"Welcome Back!\r\n   Time Away: {tsAway.ToString(@"dd\:hh\:mm\:ss")}\r\nSwitchReason: {e.Reason.ToString()}");

                        // Start the next event
                        this.ActiveEvent = new SessionEvent() { Start = this.SysUnLock.Value, EventTypeID = e.Reason };

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

                // Add Trace Listener To Update TextBox / Notification
                using (var tw = new Magic.TextBoxTraceListener(this.txtConsoleLog, this.TrayIcon)) {
                    Trace.Listeners.Add(tw);
                }

                // Add Trace Listner To Update the Text File
                using (var tw = new TextWriterTraceListener($@"{cfg.AppSettings["TracePath"]}{this.TraceFileName}", "DefaultFile")) {
                    Trace.Listeners.Add(tw);
                }

                // Save file after every Trace.Write
                Trace.AutoFlush = true;

                // Write the first line of the trace to indicate application has started 
                Trace.WriteLine($"{this.AppStart.ToString("yyyy-MM-dd HH:mm:ss")} App Starting", "SessionLaunch");

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
                var r = MessageBox.Show("Would you like to keep the app running in the background?", "", MessageBoxButtons.YesNo);
                e.Cancel = r == DialogResult.Yes;
            }
            private void tmrWork_Tick(object sender, EventArgs e) {
                this.tsWorking.Add(TimeSpan.FromMilliseconds(this.tmrWork.Interval));
                this.TrayIcon.Text = $"Time Worked: { (DateTime.Now - this.SysUnLock.Value).ToString(@"dd\:hh\:mm\:ss")}";
            }
            private void tmrBreak_Tick(object sender, EventArgs e) {
                this.tsOnBreak.Add(TimeSpan.FromMilliseconds(this.tmrBreak.Interval));
            }
        #endregion

        #region " Methods "
        private void SessionEventLog_Insert(SessionEvent e) {
                using (var DB = new SqlConnection(cfg.ConnectionStrings["BreadBoard"].ToString())) {
                    DB.Open();
                    using(var CMD = new SqlCommand("dbo.SessionEventLog_Insert", DB)) {
                        // Identify that it's a procedure otherwise you'll get a syntax error bitching aboutt not having "Exec " in front which is just, well.. INELEGANT!!
                        CMD.CommandType = CommandType.StoredProcedure;                    
                        CMD.Parameters.AddWithValue("@EventTypeID", e.EventTypeID);
                        CMD.Parameters.AddWithValue("@Start", e.Start);
                        CMD.Parameters.AddWithValue("@Ended", e.Ended);
                        CMD.Parameters.AddWithValue("@Message", e.Message);
                        // Setup Output / ReturnValue Parameters to get the newly inserted ID value
                        CMD.Parameters.Add("@NewID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        CMD.Parameters.Add("ReturnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        try {
                            CMD.ExecuteNonQuery();
                            this.ActiveEvent.ID = (int)CMD.Parameters["@NewID"].Value;
                        }
                        catch (SqlException x) {
                            Trace.WriteLine(x.ToString());
                        }
                        catch (Exception x) {
                            Trace.WriteLine(x.ToString());
                        }
                    }
                    DB.Close();
                }
            }
        #endregion

        
    }
}
