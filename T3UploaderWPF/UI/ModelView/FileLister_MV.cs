using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using T3UploaderWPF.UI.Data;
using T3UploaderWPF.UI.Model;

namespace T3UploaderWPF.UI.ModelView
{
    public class FileLister_MV : INotifyPropertyChanged
    {
        private string _path;
        private CustomFolder? _folder;
        private CustomFile? _currentFile;
        public FileListerModel? Model { get; set; }
        public string Path
        {
            get { return _path; }
            set
            {
                if (string.IsNullOrEmpty(_path) || !_path.Equals(value))
                {
                    _path = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool FolderOpened { get; set; }
        public CustomFolder? Folder
        {
            get { return _folder; }
            set { _folder = value; NotifyPropertyChanged(); }
        }
        public CustomFile? CurrentFile
        {
            get { return _currentFile; }
            set { _currentFile = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FileLister_MV(string path)
        {
            FolderOpened = false;
            _path = path;
        }

        public void GetFiles()
        {
            try
            {
                if (Model == null) throw new Exception("Model is null");
                var folder = Model.OpenFolder();
                if (folder != null)
                {
                    Folder = folder;
                    FolderOpened = true;
                    Path = Model.GetPath();
                }
            }
            catch (Exception ex)
            {
                Path = "Some error occurred";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public void UpdateFileSelected(CustomFile customFile)
        {
            CurrentFile = customFile;
        }
    }
}
