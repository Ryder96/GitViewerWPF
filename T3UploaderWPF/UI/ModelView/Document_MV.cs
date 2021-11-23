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
    public class Document_MV : INotifyPropertyChanged
    {
        private IDocumentModel _model;
        private string _content;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Content
        {
            get { return _content; }
            private set { _content = value; NotifyPropertyChanged(); }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Document_MV(IDocumentModel model)
        {
            _model = model;
            _content = string.Empty;
        }

        public void GetDocument(CustomFile file)
        {
            Content = _model.ReadContent(file);
        }
    }
}
