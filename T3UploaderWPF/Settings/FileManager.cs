using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

using T3UploaderWPF.Network;
using T3UploaderWPF.UI.Data;

namespace T3UploaderWPF.Settings
{
    public class FileManager
    {
        private readonly string _documentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private readonly string _rootFolder = "TestingUploader";
        private static readonly Lazy<FileManager> fileManager = new(() => new FileManager());

        private FileManager() { }

        public static FileManager Instance { get { return fileManager.Value; } }

        public ErrorHandler InitializeWorkspace()
        {
            var workspaceDirectory = Path.Combine(_documentFolder, _rootFolder);
            if (SettingsManager.Instance.Settings == null) return new ErrorHandler();

            var settings = SettingsManager.Instance.Settings;
            GitManager.Instance.LocalRepository = workspaceDirectory;

            var initialized = settings.Initialized;
            if (!initialized)
            {
                if (!Directory.Exists(workspaceDirectory))
                {
                    _ = Directory.CreateDirectory(workspaceDirectory);
                }
                else
                {
                    return new ErrorHandler { Success = false, ErrorMessage = "Folder exists" };
                }

                var clonePath = GitManager.Instance.Clone(workspaceDirectory, ".");
                if (string.IsNullOrEmpty(clonePath))
                    return new ErrorHandler { Success = false, ErrorMessage = "Failed to clone" };
                else
                {
                    SettingsManager.Instance.Update(!initialized);
                    return new ErrorHandler { Success = true };
                }
            }
            else
            {
                if (settings.Git == null) return new ErrorHandler { Success = false, ErrorMessage = "Git settings null" };
                if (settings.Git.Braches == null) return new ErrorHandler { Success = false, ErrorMessage = "No braches in settings" };

                foreach (var branch in settings.Git.Braches)
                {
                    GitManager.Instance.UpdateBraches(branch);
                }
            }

            return new ErrorHandler { Success = true };
        }

        public string GetPath()
        {
            return Path.Combine(_documentFolder, _rootFolder);
        }
    }
}
