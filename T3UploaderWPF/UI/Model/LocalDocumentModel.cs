using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using T3UploaderWPF.UI.Data;

namespace T3UploaderWPF.UI.Model
{
    public class LocalDocumentModel : IDocumentModel
    {
        public string ReadContent(CustomFile file)
        {
            return file.Content;
        }


    }
}
