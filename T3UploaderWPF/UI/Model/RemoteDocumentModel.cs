using LibGit2Sharp;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using T3UploaderWPF.Network;
using T3UploaderWPF.UI.Data;

namespace T3UploaderWPF.UI.Model
{
    public class RemoteDocumentModel : IDocumentModel
    {
        public string ReadContent(CustomFile file)
        {
            return GitManager.Instance.GetDiff(file.Name);
        }
    }
}
