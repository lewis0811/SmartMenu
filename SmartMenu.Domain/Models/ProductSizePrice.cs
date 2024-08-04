using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class ProductSizePrice : BaseModel
    {
        public int ProductSizePriceId { get; set; }
        public int ProductId { get; set; }
        public ProductSizeType ProductSizeType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }

        //[ForeignKey(nameof(ProductId))]
        //public Product? Product { get; set; } //
    }
}