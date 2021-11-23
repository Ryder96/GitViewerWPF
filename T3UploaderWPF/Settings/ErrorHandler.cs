using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3UploaderWPF.UI.Data
{
    public class ErrorHandler
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ExceptionMessage { get; set; }
        public Exception? Exception { get; set; }

        public string ShowError()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Success: " + Success);
            sb.AppendLine("ErrorMessage: " + ErrorMessage);
            sb.AppendLine("Exception: " + Exception);
            sb.AppendLine("Exception Message: " + ExceptionMessage);

            return sb.ToString();
        }
    }
}
