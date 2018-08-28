using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagicToolBox.LunchTray {
    /// <summary>
    /// Interaction logic for TrayPrompt.xaml
    /// </summary>
    public partial class TrayPrompt : UserControl {
        public string NotificationMessage { set; get; }
        public TrayPrompt() {
            InitializeComponent();
        }
        public void ShowDialog(string Message) {
            this.txtNotificationMessage.Text = Message;            
        }
    }
}
