using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductSizePrice : BaseModel
    {
        public int ProductSizePriceId { get; set; }
        public int ProductId { get; set; }
        public int ProductSizeId { get; set; }
        public double Price { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        [ForeignKey("ProductSizeId")]
        public ProductSize? ProductSize { get; set; }
    }
}