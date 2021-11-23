using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using T3UploaderWPF.UI.Data;
using LibGit2Sharp;
using T3UploaderWPF.Network;

namespace T3UploaderWPF.UI.Model
{
    public class FileListerModel
    {

        private readonly string choosedFolder = string.Empty;

        public FileListerModel(string path)
        {
            choosedFolder = path;
        }

        public CustomFolder? OpenFolder()
        {
            var pickFolder = new DirectoryInfo(choosedFolder);
            if (pickFolder == null) return null;

            var folder = new CustomFolder
            { Name = pickFolder.Name, Path = pickFolder.FullName };

            ReadFiles(folder, pickFolder);

            return folder;
        }

        private static void ReadFiles(CustomFolder folder, DirectoryInfo pickFolder)
        {
            var files = pickFolder.GetFiles();

            foreach (var file in files)
            {
                var content = File.ReadAllText(file.FullName);
                folder.Files.Add(new CustomFile
                { Name = file.Name, Path = file.FullName, Content = content });
            }
        }

        public string GetPath()
        {
            return choosedFolder;
        }
    }
}
