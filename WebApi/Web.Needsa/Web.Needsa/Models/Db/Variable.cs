using Web.Needsa.Models.Db.Abstract;

namespace Web.Needsa.Models.Db
{
    public class Variable : EntityDescription
    {
        public string MeasurementUnit { get; set; }
    }
}