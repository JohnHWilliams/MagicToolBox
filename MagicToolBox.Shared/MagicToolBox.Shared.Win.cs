using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MagicToolBox.Shared.Win {
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
