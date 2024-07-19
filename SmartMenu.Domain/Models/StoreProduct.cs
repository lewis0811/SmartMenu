using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreProduct : BaseModel
    {
        public int StoreProductId { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public bool IsAvailable { get; set; } = true;

        //[ForeignKey(nameof(StoreId))]
        //public Store? Store { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}