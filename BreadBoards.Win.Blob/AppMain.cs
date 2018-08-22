using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.ComponentModel;

namespace BreadBoards.Win.Blob {
    public partial class AppMain : Form {
        private string _TraceFileName;
        private string TraceFileName {
            get {
                if (string.IsNullOrEmpty(this._TraceFileName)) {
                    this._TraceFileName = Application.ExecutablePath.Substring(Application.ExecutablePath.LastIndexOf(@"\") + 1).Replace(".exe", $".TRACE.{DateTime.Now.ToString("yyyyMMdd")}.txt");
                }
                return this._TraceFileName;
            }
        }
        private enum WorkRoutine {
            LoadPictures
        }
        private enum SqlErrors {
            UniqueConstraintViolation = 2627
        }
        private int Counter { get; set; } = 1;
        // Progress Indicators 
        int ProgressMax, ProgressValue;
        DateTime ProgressStart, ProgressEnd;
        string ProgressText;
        Image ProgressImage;

        public AppMain() {
            InitializeComponent();
            // Setup Tracing
            // TraceFileName Is Unique Per Day
            using (var tw = new TextWriterTraceListener($@"C:\Windows\tracing\{this.TraceFileName}", "DefaultFile")) {
                Trace.Listeners.Add(tw);
            }
            Trace.AutoFlush = true;  // Save file after every Trace.Write
        }
        /// <summary>
        /// Event Handler For "Choose Folder" Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdPickFolder_Click(object sender, EventArgs e) {
            // Show Folder Picker Dialog 
            switch (this.fbPickFolder.ShowDialog()) {
                case DialogResult.OK:
                    // Set the TextBox.Text to the selected folder
                    this.tbSelectedPath.Text = this.fbPickFolder.SelectedPath;
                    this.ProgressStart = DateTime.Now;
                    // Run the process in the background to load pictures
                    this.bgWorker.RunWorkerAsync(WorkRoutine.LoadPictures);
                    while(this.bgWorker.IsBusy) {
                        // Update Progress Bar Maximum
                        this.pgProgress.Maximum = this.ProgressMax;
                        // Update Progress Bar Value
                        this.pgProgress.Value = this.ProgressValue;
                        // Update Progress Label
                        this.lblProgress.Text = this.ProgressText;
                        // Show the picture being loaded
                        this.pbxImageDisplay.Image = this.ProgressImage;
                        // Keep Things Moving
                        Application.DoEvents();
                    }
                    // Set the progress complete time
                    this.ProgressEnd = DateTime.Now;
                    MessageBox.Show($"{this.ProgressMax.ToString("#,##0")} files were loaded from {this.tbSelectedPath.Text} in {(this.ProgressEnd - this.ProgressStart).ToString(@"hh\:mm\:ss")}");
                    this.pgProgress.Value = 0;
                    this.lblProgress.Text = "";
                    this.pbxImageDisplay.Image = null;
                    break;
                default:
                    break;
            }
            // Force garbage collection to preserve memory resources
            GC.Collect();
        }
        private void LoadPictures(string[] FilesFound) {
            // Update Progress Bar Maximum
            this.ProgressMax = FilesFound.Length;
            // Reset the progress to 0%
            this.ProgressValue = 0;
            // Loop through all the "*.png" files
            foreach (string FileName in FilesFound) {
                // Get the File Info
                var pngFileInfo = new FileInfo(FileName);
                // Update Progress Label
                this.ProgressText = $"Loading File: {pngFileInfo.Name}";
                // Open the file and get a stream going
                using (var fsImage = new FileStream(pngFileInfo.FullName, FileMode.Open, FileAccess.Read)) {
                    // Instanciate the binary reader
                    using (var binReader = new BinaryReader(fsImage)) {
                        // File to a Blob (B.inary L.arge OB.ject)
                        var pngBlob = binReader.ReadBytes((int)fsImage.Length);
                        // Instanciate SQL Connection
                        using (var DB = new SqlConnection(ConfigurationManager.ConnectionStrings["Local"].ConnectionString)) {
                            // Open Connection
                            DB.Open();
                            // Create The SqlCommand
                            using (var CMD = new SqlCommand("blob.PictureData_Insert", DB)) {
                                // Identify that it's a procedure otherwise you'll get a syntax error
                                CMD.CommandType = CommandType.StoredProcedure;
                                CMD.Parameters.Add("@FileSource", SqlDbType.NVarChar).Value = pngFileInfo.FullName;
                                CMD.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = pngFileInfo.Name;
                                CMD.Parameters.Add("@FileCreated", SqlDbType.DateTime).Value = pngFileInfo.CreationTime;
                                CMD.Parameters.Add("@FileModified", SqlDbType.DateTime).Value = pngFileInfo.LastWriteTime;
                                this.ProgressImage = Image.FromStream(fsImage);
                                CMD.Parameters.Add("@FileWidthPx", SqlDbType.Int).Value = this.ProgressImage.Width;
                                CMD.Parameters.Add("@FileHeightPx", SqlDbType.Int).Value = this.ProgressImage.Height;                                
                                // Finally Add The Blob 
                                CMD.Parameters.Add("@FileBlob", SqlDbType.Image, pngBlob.Length).Value = pngBlob;
                                try {
                                    CMD.ExecuteNonQuery();  // Execute The Command
                                }
                                catch (SqlException x) {
                                    switch (x.Number) {
                                        case (int)SqlErrors.UniqueConstraintViolation:
                                            // Already Added This File No Need To Fail The Whole Process
                                            break;
                                    }
                                    Trace.WriteLine(x.ToString());
                                }
                            }
                            // Close Connection
                            DB.Close();
                        }
                    }
                }
                // Update Progress Completion
                this.ProgressValue++;
                this.ProgressText = "Files Loaded";
            }
        }
        /// <summary>
        /// This handles the click of the "Get Image From DB" button 
        /// It selects an individual record from the database and loads the image into the image box form control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdGetImageFromDB_Click(object sender, EventArgs e) {
            using (var DB = new SqlConnection(ConfigurationManager.ConnectionStrings["Local"].ConnectionString)) {
                // Open Connection
                DB.Open();
                // Create Transaction
                using (var txGetFile = DB.BeginTransaction("GetPictureFileStream")) {
                    // Create The SqlCommand
                    using (var CMD = new SqlCommand($"blob.PictureData_Select", DB, txGetFile)) {
                        CMD.CommandType = CommandType.StoredProcedure;
                        CMD.Parameters.AddWithValue("@RowID", Counter);
                        using (var DR = CMD.ExecuteReader()) {
                            while (DR.Read()) {
                                // Pull the file by s 
                                using (var sfs = new SqlFileStream(DR["FilePath"].ToString(), (byte[])DR["FileContext"], FileAccess.Read)) {
                                    // Display The Image On The Form
                                    this.pbxImageDisplay.Image = Image.FromStream(sfs);
                                }
                            }
                        }
                    }
                    // Commit the transaction
                    txGetFile.Commit();
                }
                // Close Connection
                DB.Close();
            }
            // Increment Counter
            this.Counter++;
        }
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e) {
            var work = (BackgroundWorker)sender;
            switch((WorkRoutine)e.Argument) {
                case WorkRoutine.LoadPictures:                    
                    this.LoadPictures(Directory.GetFiles(this.tbSelectedPath.Text, "*.png", SearchOption.TopDirectoryOnly));
                    break;
            }
        }
    }
}
