using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductSizePrice : BaseModel
    {
        public int ProductSizePriceId { get; set; }
        public int ProductId { get; set; }
        public ProductSizeType ProductSizeType { get; set; }
        public double Price { get; set; }

        //[ForeignKey(nameof(ProductId))]
        //public Product? Product { get; set; } //
    }
}