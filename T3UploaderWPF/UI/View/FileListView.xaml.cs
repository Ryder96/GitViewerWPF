using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

using T3UploaderWPF.UI.Data;
using T3UploaderWPF.UI.ModelView;

namespace T3UploaderWPF.UI.View
{
    /// <summary>
    /// Interaction logic for FileListView.xaml
    /// </summary>
    public partial class FileListView : Page
    {
        private readonly FileLister_MV _modelView;
        public ObservableCollection<CustomFile> DataSource { get; set; } = new();

        public FileListView(FileLister_MV modelView)
        {
            _modelView = modelView;
            _modelView.Model = new Model.FileListerModel(_modelView.Path);
            _modelView.PropertyChanged += ModelView_PropertyChanged;

            InitializeComponent();
            InitializeView();
        }


        private void InitializeView()
        {
            lv_files.ItemsSource = DataSource;
            if (!_modelView.FolderOpened)
                _modelView.GetFiles();
        }

        private void ModelView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) && e.PropertyName.Equals("Folder") && _modelView.Folder != null)
            {
                foreach (var file in _modelView.Folder.Files)
                {
                    DataSource.Add(file);
                }
            }
        }

        private void Lv_files_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var s = sender as ListView;

            if (s?.SelectedItem is CustomFile itemSelected)
            {
                _modelView.UpdateFileSelected(itemSelected);
            }
        }
    }
}
