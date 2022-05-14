using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Daekage.Core.Models
{
    public class NoticeModel
    {
        [JsonProperty("Id")]
        public string Key { get; set; }

        public string Writer { get; set; }

        public string Date { get; set; }

        public List<string> Files { get; set; }

        public bool IsFilesEmpty => Files?.Count > 0;

        public string Text { get; set; }
    }
}
