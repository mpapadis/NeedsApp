using System.ComponentModel.DataAnnotations;

namespace Web.Needsa.Models.Db.Abstract
{
    public abstract class Entity
    {
        [Key]
        public int Id { get; set; }
    }

    public abstract class EntityDescription : Entity
    {
        public string Description { get; set; }
    }
}
