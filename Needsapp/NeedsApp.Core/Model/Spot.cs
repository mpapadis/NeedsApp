using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeedsApp.Core.Model
{
    [PropertyChanged.ImplementPropertyChanged]
    public class Spot: BaseModel
    {

        [JsonProperty("id")]
        public new int ID { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("position")]
        public Position Position { get; set; }

        [JsonProperty("sensors")]
        public int Sensors { get; set; }

        [JsonProperty("status")]
        public Status WaterStatus { get; set; }

        [JsonProperty("accessible")]
        public bool Accessible { get; set; }

        public enum Status
        {
            open,
            closed
        }
    }
   
}
