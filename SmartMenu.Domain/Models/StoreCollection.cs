using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreCollection : BaseModel
    {
        public int StoreCollectionID { get; set; }
        public int StoreID { get; set; }
        public int CollectionID { get; set; }

        [ForeignKey(nameof(CollectionID))]
        public Collection? Collection { get; set; }

        //[ForeignKey(nameof(StoreID))]
        //public Store? Store { get; set; }
    }
}