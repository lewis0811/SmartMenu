using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreCollection : BaseModel
    {
        public int StoreCollectionId { get; set; }
        public int StoreId { get; set; }
        public int CollectionId { get; set; }

        [ForeignKey(nameof(CollectionId))]
        public Collection? Collection { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store? Store { get; set; } //
    }
}