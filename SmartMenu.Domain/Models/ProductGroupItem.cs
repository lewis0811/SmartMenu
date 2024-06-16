using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductGroupItem : BaseModel
    {
        public int ProductGroupItemId { get; set; }
        [Required]
        public int ProductGroupId { get; set; }
        [Required]
        public int ProductSizePriceId { get; set; }

        [ForeignKey("ProductSizePriceId")]
        public ProductSizePrice? ProductSizePrice { get; set; }

        [ForeignKey("ProductGroupId")]
        public ProductGroup? ProductGroup { get; set; }
    }
}