using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3UploaderWPF.Settings
{
    public partial class Settings
    {
        [JsonProperty("initialized")]
        public bool Initialized { get; set; }

        [JsonProperty("git")]
        public SettingsGit? Git { get; set; }
    }

    public partial class Account
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("token")]
        public string? Token { get; set; }
    }

    public partial class SettingsGit
    {
        [JsonProperty("account")]
        public Account? Account { get; set; }

        [JsonProperty("author")]
        public Author? Author { get; set; }

        [JsonProperty("remote")]
        public string? Remote { get; set; }

        [JsonProperty("braches")]
        public string[]? Braches { get; set; }
    }

    public partial class Author
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("email")]
        public string? Email { get; set; }
    }
}
