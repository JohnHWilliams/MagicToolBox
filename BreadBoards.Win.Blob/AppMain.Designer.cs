namespace BreadBoards.Win.Blob {
    partial class AppMain {
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
            this.fbPickFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.pbxImageDisplay = new System.Windows.Forms.PictureBox();
            this.pnlActionControls = new System.Windows.Forms.Panel();
            this.cmdPickFolder = new System.Windows.Forms.Button();
            this.tbSelectedPath = new System.Windows.Forms.TextBox();
            this.cmdGetImageFromDB = new System.Windows.Forms.Button();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.stsBottom = new System.Windows.Forms.StatusStrip();
            this.pgProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.ctMainSplitView = new System.Windows.Forms.SplitContainer();
            this.dbsPictureData = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbxImageDisplay)).BeginInit();
            this.pnlActionControls.SuspendLayout();
            this.stsBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctMainSplitView)).BeginInit();
            this.ctMainSplitView.Panel1.SuspendLayout();
            this.ctMainSplitView.Panel2.SuspendLayout();
            this.ctMainSplitView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dbsPictureData)).BeginInit();
            this.SuspendLayout();
            // 
            // pbxImageDisplay
            // 
            this.pbxImageDisplay.BackColor = System.Drawing.SystemColors.Control;
            this.pbxImageDisplay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbxImageDisplay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxImageDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbxImageDisplay.Location = new System.Drawing.Point(0, 0);
            this.pbxImageDisplay.Name = "pbxImageDisplay";
            this.pbxImageDisplay.Size = new System.Drawing.Size(1244, 654);
            this.pbxImageDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxImageDisplay.TabIndex = 2;
            this.pbxImageDisplay.TabStop = false;
            // 
            // pnlActionControls
            // 
            this.pnlActionControls.Controls.Add(this.cmdPickFolder);
            this.pnlActionControls.Controls.Add(this.tbSelectedPath);
            this.pnlActionControls.Controls.Add(this.cmdGetImageFromDB);
            this.pnlActionControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActionControls.Location = new System.Drawing.Point(0, 0);
            this.pnlActionControls.Name = "pnlActionControls";
            this.pnlActionControls.Size = new System.Drawing.Size(1244, 63);
            this.pnlActionControls.TabIndex = 5;
            // 
            // cmdPickFolder
            // 
            this.cmdPickFolder.Location = new System.Drawing.Point(3, 6);
            this.cmdPickFolder.Name = "cmdPickFolder";
            this.cmdPickFolder.Size = new System.Drawing.Size(115, 22);
            this.cmdPickFolder.TabIndex = 0;
            this.cmdPickFolder.Text = "Chose Folder";
            this.cmdPickFolder.UseVisualStyleBackColor = true;
            this.cmdPickFolder.Click += new System.EventHandler(this.cmdPickFolder_Click);
            // 
            // tbSelectedPath
            // 
            this.tbSelectedPath.Location = new System.Drawing.Point(121, 7);
            this.tbSelectedPath.Name = "tbSelectedPath";
            this.tbSelectedPath.Size = new System.Drawing.Size(437, 20);
            this.tbSelectedPath.TabIndex = 1;
            // 
            // cmdGetImageFromDB
            // 
            this.cmdGetImageFromDB.Location = new System.Drawing.Point(3, 32);
            this.cmdGetImageFromDB.Name = "cmdGetImageFromDB";
            this.cmdGetImageFromDB.Size = new System.Drawing.Size(115, 22);
            this.cmdGetImageFromDB.TabIndex = 3;
            this.cmdGetImageFromDB.Text = "Get Image From DB";
            this.cmdGetImageFromDB.UseVisualStyleBackColor = true;
            this.cmdGetImageFromDB.Click += new System.EventHandler(this.cmdGetImageFromDB_Click);
            // 
            // tblMain
            // 
            this.tblMain.AutoScroll = true;
            this.tblMain.AutoSize = true;
            this.tblMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tblMain.Location = new System.Drawing.Point(12, 82);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 1;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(0, 0);
            this.tblMain.TabIndex = 4;
            // 
            // stsBottom
            // 
            this.stsBottom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pgProgress,
            this.lblProgress});
            this.stsBottom.Location = new System.Drawing.Point(0, 699);
            this.stsBottom.Name = "stsBottom";
            this.stsBottom.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.stsBottom.Size = new System.Drawing.Size(1244, 22);
            this.stsBottom.TabIndex = 6;
            this.stsBottom.Text = "statusStrip1";
            // 
            // pgProgress
            // 
            this.pgProgress.Name = "pgProgress";
            this.pgProgress.Size = new System.Drawing.Size(100, 16);
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = false;
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(300, 17);
            // 
            // ctMainSplitView
            // 
            this.ctMainSplitView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctMainSplitView.Location = new System.Drawing.Point(0, 0);
            this.ctMainSplitView.Name = "ctMainSplitView";
            this.ctMainSplitView.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ctMainSplitView.Panel1
            // 
            this.ctMainSplitView.Panel1.Controls.Add(this.pnlActionControls);
            // 
            // ctMainSplitView.Panel2
            // 
            this.ctMainSplitView.Panel2.AutoScroll = true;
            this.ctMainSplitView.Panel2.Controls.Add(this.pbxImageDisplay);
            this.ctMainSplitView.Size = new System.Drawing.Size(1244, 721);
            this.ctMainSplitView.SplitterDistance = 63;
            this.ctMainSplitView.TabIndex = 7;
            // 
            // dbsPictureData
            // 
            this.dbsPictureData.AllowNew = false;
            // 
            // AppMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1244, 721);
            this.Controls.Add(this.stsBottom);
            this.Controls.Add(this.tblMain);
            this.Controls.Add(this.ctMainSplitView);
            this.Name = "AppMain";
            this.Text = "App Main";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.pbxImageDisplay)).EndInit();
            this.pnlActionControls.ResumeLayout(false);
            this.pnlActionControls.PerformLayout();
            this.stsBottom.ResumeLayout(false);
            this.stsBottom.PerformLayout();
            this.ctMainSplitView.Panel1.ResumeLayout(false);
            this.ctMainSplitView.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ctMainSplitView)).EndInit();
            this.ctMainSplitView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dbsPictureData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog fbPickFolder;
        private System.Windows.Forms.PictureBox pbxImageDisplay;
        private System.Windows.Forms.Panel pnlActionControls;
        private System.Windows.Forms.Button cmdPickFolder;
        private System.Windows.Forms.TextBox tbSelectedPath;
        private System.Windows.Forms.Button cmdGetImageFromDB;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.StatusStrip stsBottom;
        private System.Windows.Forms.ToolStripStatusLabel lblProgress;
        private System.Windows.Forms.ToolStripProgressBar pgProgress;
        private System.Windows.Forms.SplitContainer ctMainSplitView;
        private System.Windows.Forms.BindingSource dbsPictureData;
    }
}

