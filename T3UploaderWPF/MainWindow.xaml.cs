using LibGit2Sharp;
using LibGit2Sharp.Handlers;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

using T3UploaderWPF.Network;
using T3UploaderWPF.Settings;
using T3UploaderWPF.UI.Data;
using T3UploaderWPF.UI.Model;
using T3UploaderWPF.UI.ModelView;
using T3UploaderWPF.UI.View;

namespace T3UploaderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly FileLister_MV _fileLister;
        private readonly Document_MV _localDocument;
        private readonly Document_MV _remoteDocument;

        public MainWindow()
        {
            var checkSettings = SettingsManager.Instance.ImportSettings();
            if (!checkSettings.Success)
                Close();
            var checkInitialization = FileManager.Instance.InitializeWorkspace();
            if (!checkInitialization.Success)
                Close();

            _fileLister = new FileLister_MV(FileManager.Instance.GetPath());
            _localDocument = new Document_MV(new LocalDocumentModel());
            _remoteDocument = new Document_MV(new RemoteDocumentModel());
            SubscribeToEvents();

            InitializeComponent();
            tb_folderPath.Text = FileManager.Instance.GetPath();
            localDocumentFrame.NavigationService.Navigate(new DocumentView(_localDocument));
            remoteDocumentFrame.NavigationService.Navigate(new DocumentView(_remoteDocument));
            treeViewFrame.NavigationService.Navigate(new FileListView(_fileLister));
        }


        private void SubscribeToEvents()
        {
            _fileLister.PropertyChanged += FileLister_PropertyChanged;
        }

        private void FileLister_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName))
            {
                if (e.PropertyName.Equals("Path"))
                {
                    tb_folderPath.Text = _fileLister.Path;
                    GitManager.Instance.LocalRepository = _fileLister.Path;
                }
                if (e.PropertyName.Equals("CurrentFile") && _fileLister.CurrentFile != null)
                {
                    _localDocument.GetDocument(_fileLister.CurrentFile);
                    _remoteDocument.GetDocument(_fileLister.CurrentFile);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_fileLister.Path))
                Process.Start("explorer.exe", _fileLister.Path);
        }

        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            string message = string.Empty;
            ErrorHandler? gitResponse = null;

            var hasUncommittedChanges = GitManager.Instance.LastCommitStatus();

            var dialog = new CommitDialog();
            if (hasUncommittedChanges.Dirty)
            {
                dialog.SetReadOnly(hasUncommittedChanges.Message);
            }

            var result = dialog.ShowDialog();
            if (result == true && !dialog.ReadOnly)
            {
                if (!string.IsNullOrEmpty(dialog.Message))
                    message = dialog.Message;
                else
                    message = $"Update changes at {DateTime.UtcNow.ToLongTimeString()}";
            }
            else if (result == false)
            {
                return;
            }

            switch (button.Uid)
            {
                case "master":
                    gitResponse = GitManager.Instance.PushToRemote(message, "master");
                    break;
                case "dev":
                    gitResponse = GitManager.Instance.PushToRemote(message, "dev");
                    break;
                case "prod":
                    gitResponse = GitManager.Instance.PushToRemote(message, "prod");
                    break;
            }

            if (gitResponse?.Success == true)
            {
                MessageBox.Show("Successfully update at " + button.Uid);
            }
            else if (gitResponse != null)
            {
                MessageBox.Show(gitResponse?.ShowError(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
