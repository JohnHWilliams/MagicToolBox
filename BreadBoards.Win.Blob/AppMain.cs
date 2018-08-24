using System;
using System.IO;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Magic=MagicToolBox.Shared.Win;
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
            private int Counter { set; get; } = 1;
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
                        var SearchPattern = Magic.Prompt.ShowDialog("Search Pattern", "Enter a value", "*.tif*");
                        // Set ProgresStart Time
                        this.ProgressStart = DateTime.Now;
                        // Run the method via Task and run the picture loads in the background
                        using (var t = Task.Factory.StartNew(() => this.UploadPictures(Directory.GetFiles(this.tbSelectedPath.Text, SearchPattern, SearchOption.TopDirectoryOnly)))) {
                            while (!t.IsCompleted) {
                                // Update Progress Information
                                this.pgProgress.Value = this.ProgressValue;
                                this.lblProgress.Text = $"Loading File: {this.ProgressText}";
                                this.lblDetails.Text = this.ProgressText;
                                // Show the picture being loaded
                                this.pbxImageDisplay.Image = this.ProgressImage;
                                //// Keep Things Moving Otherwise The Progress Bar Doesn't Update Properly
                                //Application.DoEvents();
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
                // Loop through all the picture files
                foreach (var FileName in FilesFound) {
                    // Get the File Info
                    var imgInfo = new FileInfo(FileName);
                    // Setup a place to put the data
                    byte[] blob;
                    // Update Progress
                    this.ProgressText = imgInfo.Name;
                    // Open the file and get a stream going
                    using (var fsSource = new FileStream(imgInfo.FullName, FileMode.Open, FileAccess.Read)) {
                        // Instanciate Image Object
                        using (var img = Image.FromStream(fsSource)) {
                            // Page Out Tiff Files By Individual Page ( Index Is ONE 1 BASED!! )
                            for (var i = 0; i < img.GetFrameCount(FrameDimension.Page); i++) {
                                // Instanciate a destination to save the new png image to
                                using (var ms = new MemoryStream()) {
                                    // Select the page per this iteration
                                    img.SelectActiveFrame(FrameDimension.Page, i);
                                    // Save to the memory as PNG
                                    img.Save(ms, ImageFormat.Png);
                                    blob = ms.ToArray();
                                    // Put it in the database!
                                    this.AddPictureData(imgInfo, img, blob, i, ImageFormat.Png.ToString().ToUpper());
                                    // Set so the image will show in the PictureBox control
                                    this.ProgressImage = Image.FromStream(ms);  // Can't use img because it will bitch about the image being in use 
                                }
                            }
                        }
                    }                        
                    // Update Progress Completion
                    this.ProgressValue++;
                    this.ProgressText = "Files Loaded";
                }            
            }
            private void AddPictureData(FileInfo imgInfo, Image img, byte[] Blob, int PageIndex, string BlobType) {
                // Instanciate SQL Connection
                using (var DB = new SqlConnection(cfg.ConnectionStrings["Local"].ConnectionString)) {
                    // Open Connection
                    DB.Open();
                    // Create The SqlCommand
                    using (var CMD = new SqlCommand("blob.PictureData_Insert", DB)) {
                        // Identify that it's a procedure otherwise you'll get a syntax error bitching aboutt not having "Exec " in front which is just, well.. INELEGANT!!
                        CMD.CommandType = CommandType.StoredProcedure;
                        CMD.Parameters.AddWithValue("@RowCreator", $@"{Environment.UserDomainName}\{Environment.UserName}");
                        CMD.Parameters.AddWithValue("@RowCreatorHost", Dns.GetHostAddresses(Dns.GetHostName()).First(addr => addr.AddressFamily == AddressFamily.InterNetwork).ToString());
                        CMD.Parameters.AddWithValue("@RowCreatorHostName", Dns.GetHostName());
                        CMD.Parameters.AddWithValue("@FileSource", imgInfo.FullName);
                        CMD.Parameters.AddWithValue("@FileName", imgInfo.Name);
                        CMD.Parameters.AddWithValue("@FileCreated", imgInfo.CreationTime);
                        CMD.Parameters.AddWithValue("@FileModified", imgInfo.LastWriteTime);
                        CMD.Parameters.AddWithValue("@FileWidthPx", img.Width);
                        CMD.Parameters.AddWithValue("@FileHeightPx", img.Height);
                        CMD.Parameters.AddWithValue("@FilePageCount", img.GetFrameCount(FrameDimension.Page));
                        CMD.Parameters.AddWithValue("@FilePageIndex", PageIndex);
                        CMD.Parameters.AddWithValue("@FileBlob", Blob);
                        CMD.Parameters.AddWithValue("@FileBlobType", BlobType);
                        try {
                            CMD.ExecuteNonQuery();  // Execute The Command
                        }
                        catch (SqlException x) {
                            switch (x.Number) {
                                case (int)SqlErrors.UniqueConstraintViolation: // Already Added This File No Need To Fail The Whole Process
                                    Trace.WriteLine(x.ToString());
                                    break;
                                default:
                                    Trace.WriteLine(x.ToString());
                                    throw (x);  //TODO: Maybe just tracing the error will be fine?
                            }
                        }
                    }
                    // Close Connection
                    DB.Close();
                }
            }
            private void GetPictureData() {
                using (var DB = new SqlConnection(cfg.ConnectionStrings["Local"].ConnectionString)) {
                    // Open Connection
                    DB.Open();
                    // Create Transaction
                    using (var txGetFile = DB.BeginTransaction("GetPictureFileStream")) {
                        try {
                            // Create The SqlCommand
                            using (var CMD = new SqlCommand($"blob.PictureData_Select", DB, txGetFile)) {
                                CMD.CommandType = CommandType.StoredProcedure;
                                CMD.Parameters.AddWithValue("@RowID", this.Counter);
                                using (var DR = CMD.ExecuteReader()) {
                                    while (DR.Read()) {
                                        // Pull the file via SqlFileStream
                                        using (var sfs = new SqlFileStream(DR["PathName"].ToString(), (byte[])DR["FileContext"], FileAccess.Read)) {
                                            this.ttDetail.IsBalloon = true;
                                            this.ttDetail.ToolTipTitle = "DB Image Details";
                                            // Display The Image On The Form
                                            this.pbxImageDisplay.Invalidate(true);
                                            this.pbxImageDisplay.Image = Image.FromStream(sfs);
                                            // Now show details on the form                                            
                                            this.lblDetails.Invalidate(true);
                                            this.lblDetails.Text = $"{DR["FileName"].ToString().Split(new char[] { '.' })[0]}.{(int)DR["FilePageIndex"] + 1:00}-{DR["FilePageCount"]:00}.{DR["FileBlobType"]}";
                                            var sb = new System.Text.StringBuilder();
                                            for (var i = 0; i < DR.FieldCount; i++) {
                                                sb.Append($"{DR.GetName(i)}: {DR[i]}\r\n");
                                            }
                                            this.ttDetail.SetToolTip(this.lblDetails, sb.ToString());
                                            this.ttDetail.SetToolTip(this.pbxImageDisplay, sb.ToString());
                                        }
                                    }
                                }
                            }
                            // Commit the transaction
                            txGetFile.Commit();
                        }
                        catch (Exception x) {
                            txGetFile.Rollback("GetPictureFileStream");
                            Trace.WriteLine(x.ToString());
                        }
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
