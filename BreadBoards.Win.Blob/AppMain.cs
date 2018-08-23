using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using cfg = System.Configuration.ConfigurationManager;

namespace BreadBoards.Win.Blob {
    public partial class AppMain : Form {

        #region " Types "
            private enum WorkRoutine {
                LoadPictures
            }
            private enum SqlErrors {
                UniqueConstraintViolation = 2627
            }
        #endregion

        #region " Properties "
            private string _TraceFileName;
            private string TraceFileName {
                get {
                    if (string.IsNullOrEmpty(this._TraceFileName)) {
                        this._TraceFileName = string.Format(cfg.AppSettings["TraceFileNameFormat"], DateTime.Now);
                    }
                    return this._TraceFileName;
                }
            }
            private int Counter { set; get; } = 1838;
            // Progress Indicators
            private int ProgressMax { set; get; }
            private int ProgressValue { set; get; }
            private string ProgressText { set; get; }
            private Image ProgressImage { set; get; }
            private DateTime ProgressStart { set; get; }
            private DateTime ProgressEnd { set; get; }
        #endregion
        
        #region " Constructors "
            /// <summary>
            /// Default Constructor / Entry Point
            /// </summary>
            public AppMain() {
                this.InitializeComponent();
                // Setup Tracing
                // TraceFileName Is Unique Per Day
                using (var tw = new TextWriterTraceListener($@"{cfg.AppSettings["TracePath"]}{this.TraceFileName}", "DefaultFile")) {
                    Trace.Listeners.Add(tw);
                }
                Trace.AutoFlush = true;  // Save file after every Trace.Write
            }
        #endregion

        #region " Events "
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
                        // Set ProgresStart Time
                        this.ProgressStart = DateTime.Now;
                        // Run the method via Task and run the picture loads in the background
                        using (var t = Task.Factory.StartNew(() => this.UploadPictures(Directory.GetFiles(this.tbSelectedPath.Text, "*.tif*", SearchOption.TopDirectoryOnly)))) {
                            while (!t.IsCompleted) {
                                // Update Progress Bar Maximum
                                this.pgProgress.Maximum = ProgressMax;
                                // Update Progress Bar Value
                                this.pgProgress.Value = ProgressValue;
                                // Update Progress Label
                                this.lblProgress.Text = ProgressText;
                                // Show the picture being loaded
                                this.pbxImageDisplay.Image = ProgressImage;
                                // Keep Things Moving
                                Application.DoEvents();
                            }
                        }
                        // Set the progress complete time
                        this.ProgressEnd = DateTime.Now;
                        // Indicate Completion w/ Stats
                        MessageBox.Show($"{this.ProgressMax.ToString("#,##0")} files were loaded from {this.tbSelectedPath.Text} in {(this.ProgressEnd - this.ProgressStart).ToString(@"hh\:mm\:ss")}");
                        // Reset progress indicators
                        this.pgProgress.Value = 0;
                        this.lblProgress.Text = "";
                        this.pbxImageDisplay.Image = null;
                        break;
                    default:
                        break;
                }
                // Force garbage collection to release memory resources
                GC.Collect();
            }
            /// <summary>
            /// This handles the click of the "Get Image From DB" button 
            /// It selects an individual record from the database and loads the image into the image box form control.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void cmdGetImageFromDB_Click(object sender, EventArgs e) {
                this.GetPictureData();
            }
        #endregion

        #region " Methods "
            private void UploadPictures(string[] FilesFound) {
                // Update Progress Bar Maximum
                this.ProgressMax = FilesFound.Length;
                // Reset the progress to 0%
                this.ProgressValue = 0;
                // Loop through all the "*.png" files
                foreach (var FileName in FilesFound) {
                    // Get the File Info
                    var img = new FileInfo(FileName);
                    // Setup a place to put the data
                    byte[] blob;
                    // Update Progress Label
                    this.ProgressText = $"Loading File: {img.Name}";
                    // Open the file and get a stream going
                    using (var fsImage = new FileStream(img.FullName, FileMode.Open, FileAccess.Read)) {
                        // Set so the image will show in the PictuureBox control
                        this.ProgressImage = Image.FromStream(fsImage);
                        // Instanciate the binary reader
                        using (var binReader = new BinaryReader(fsImage)) {
                            // File to a Blob(B.inary L.arge OB.ject)
                            blob = binReader.ReadBytes((int)fsImage.Length);
                        }
                    }
                    // Instanciate SQL Connection
                    using (var DB = new SqlConnection(cfg.ConnectionStrings["Local"].ConnectionString)) {
                        // Open Connection
                        DB.Open();
                        // Create The SqlCommand
                        using (var CMD = new SqlCommand("blob.PictureData_Insert", DB)) {
                            // Identify that it's a procedure otherwise you'll get a syntax error
                            CMD.CommandType = CommandType.StoredProcedure;
                            CMD.Parameters.AddWithValue("@FileSource", img.FullName);
                            CMD.Parameters.AddWithValue("@FileName", img.Name);
                            CMD.Parameters.AddWithValue("@FileCreated", img.CreationTime);
                            CMD.Parameters.AddWithValue("@FileModified", img.LastWriteTime);
                            CMD.Parameters.AddWithValue("@FileWidthPx", this.ProgressImage.Width);
                            CMD.Parameters.AddWithValue("@FileHeightPx", this.ProgressImage.Height);
                            // Finally Add The Blob
                            CMD.Parameters.AddWithValue("@FileBlob", blob);
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
                    // Update Progress Completion
                    this.ProgressValue++;
                    this.ProgressText = "Files Loaded";
                }            
            }
            private void GetPictureData() {
                using (var DB = new SqlConnection(cfg.ConnectionStrings["Local"].ConnectionString)) {
                    // Open Connection
                    DB.Open();
                    // Create Transaction
                    using (var txGetFile = DB.BeginTransaction("GetPictureFileStream")) {
                        // Create The SqlCommand
                        using (var CMD = new SqlCommand($"blob.PictureData_Select", DB, txGetFile)) {
                            CMD.CommandType = CommandType.StoredProcedure;
                            CMD.Parameters.AddWithValue("@RowID", this.Counter);
                            using (var DR = CMD.ExecuteReader()) {
                                while (DR.Read()) {
                                    // Pull the file by s 
                                    using (var sfs = new SqlFileStream(DR["PathName"].ToString(), (byte[])DR["FileContext"], FileAccess.Read)) {
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
        #endregion

    }
}
