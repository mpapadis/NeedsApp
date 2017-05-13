using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Authentication;
using Web.Needsa.Models.Db.Abstract;


namespace Web.Needsa.Models.Db
{
    public class ArduinoStationVariable : Entity
    {
        public decimal ValueCaptured { get; set; }
        public DateTimeOffset DateCaptured { get; set; }
        public int ArduinoStationId { get; set; }
        public int VariableId { get; set; }

        [ForeignKey("ArduinoStationId")]
        public ArduinoStation ArduinoStation { get; set; }
        [ForeignKey("VariableId")]
        public Variable Variable { get; set; }
    }
}