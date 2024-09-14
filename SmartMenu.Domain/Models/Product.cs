using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Product : BaseModel
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string? ProductDescription { get; set; }
        public ProductPriceCurrency ProductPriceCurrency { get; set; }
        public string? ProductImgPath { get; private set; }
        //public string? ProductLogoPath { get; private set; }

        //[ForeignKey(nameof(CategoryId))]
        //public Category? Category { get; set; } //

        public ICollection<ProductSizePrice>? ProductSizePrices { get; set; }
    }
}