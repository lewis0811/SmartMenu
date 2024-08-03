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
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(ProductGroupId))]
        public ProductGroup? ProductGroup { get; set; } //
    }
}