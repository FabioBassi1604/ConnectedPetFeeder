using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartphoneApp.Models
{
    public class FillCiotolaRequest
    {
        [JsonProperty("cmd")]
        public string Cmd { get; set; }
    }
}
