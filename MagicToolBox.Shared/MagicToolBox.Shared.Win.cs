using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace MagicToolBox.Shared.Win {
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
    public static class Prompt {
        public static string ShowDialog(string Caption, string Message, string DefaultValue) {
            var UserInput = new Form() {
                Width = 100
               ,Height = 90
               ,Text = Caption
               ,FormBorderStyle = FormBorderStyle.FixedDialog
               ,StartPosition = FormStartPosition.CenterScreen
               ,MinimizeBox = false
               ,MaximizeBox = false
               ,SizeGripStyle = SizeGripStyle.Hide
            };
            var lblMessage = new Label() { Top = 5,  Left =  0, Width = 120, Height = 15, Text = Message, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold) };
            var txtValue = new TextBox() { Top = 20, Left = 20, Width =  45, Height = 20, Text = DefaultValue, TextAlign = HorizontalAlignment.Center };
            var OK = new Button()        { Top = 20, Left = 66, Width =  35, Height = 20, Text = "Ok", DialogResult = DialogResult.OK };
            OK.Click += (sender, e) => { UserInput.Close(); };
            UserInput.Controls.Add(lblMessage);
            UserInput.Controls.Add(txtValue);
            UserInput.Controls.Add(OK);
            UserInput.AcceptButton = OK;
            return UserInput.ShowDialog() == DialogResult.OK ? txtValue.Text : DefaultValue;
        }
    }
}
