using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartphoneApp.Models
{
    public class AskCiotolaRequest
    {
        [JsonProperty("dose")]
        public int Dose { get; set; }
    }
}
