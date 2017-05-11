using SQLite.Net.Attributes;

namespace NeedsApp.Core.Model
{
    public class BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
    }
}
