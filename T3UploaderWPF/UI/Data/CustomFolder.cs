using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3UploaderWPF.UI.Data
{
    public class CustomFolder
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public ObservableCollection<CustomFile> Files { get; set; } = new ObservableCollection<CustomFile>();
    }
}
