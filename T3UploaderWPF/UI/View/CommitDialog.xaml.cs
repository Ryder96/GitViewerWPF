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
using System.Windows.Shapes;

namespace T3UploaderWPF.UI.View
{
    /// <summary>
    /// Interaction logic for CommitDialog.xaml
    /// </summary>
    public partial class CommitDialog : Window
    {
        public string Message { get; set; } = string.Empty;
        public CommitDialog()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Message = tb_message.Text;
        }
    }
}
