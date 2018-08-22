namespace BreadBoards.Win.Tray {
    partial class App_Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(App_Main));
            this.txtConsoleLog = new System.Windows.Forms.TextBox();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.tmrBreaks = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // txtConsoleLog
            // 
            this.txtConsoleLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsoleLog.Location = new System.Drawing.Point(0, 0);
            this.txtConsoleLog.Multiline = true;
            this.txtConsoleLog.Name = "txtConsoleLog";
            this.txtConsoleLog.ReadOnly = true;
            this.txtConsoleLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsoleLog.Size = new System.Drawing.Size(784, 561);
            this.txtConsoleLog.TabIndex = 0;
            // 
            // TrayIcon
            // 
            this.TrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.TrayIcon.BalloonTipTitle = "Break Timer";
            this.TrayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("TrayIcon.Icon")));
            this.TrayIcon.Text = "TrayIconText";
            this.TrayIcon.Visible = true;
            this.TrayIcon.DoubleClick += new System.EventHandler(this.TrayIcon_DoubleClick);
            // 
            // tmrBreaks
            // 
            this.tmrBreaks.Interval = 1000;
            // 
            // App_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.txtConsoleLog);
            this.Name = "App_Main";
            this.Text = "BreadBoards.Win.Tray";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.App_Main_FormClosing);
            this.Load += new System.EventHandler(this.App_Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtConsoleLog;
        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.Timer tmrBreaks;
    }
}

