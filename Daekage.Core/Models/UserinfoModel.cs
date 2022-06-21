using System;
using System.ComponentModel.Design.Serialization;
using Newtonsoft.Json;

namespace Daekage.Core.Models
{
    public class UserinfoModel
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("picture")]
        public string Picture { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("email_verified")]
        public bool EmailVerified { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("hd")]
        public string Domain { get; set; }
    }
}
