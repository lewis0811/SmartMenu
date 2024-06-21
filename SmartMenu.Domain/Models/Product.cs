using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Product : BaseModel
    {
        public int ProductID { get; set; }
        public int BrandID { get; set; }
        public int CategoryID { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string ProductDescription { get; set; } = string.Empty;

        //[Range(1, int.MaxValue)]
        //public double ProductPrice { get; set; } = 1;

        //[ForeignKey(nameof(BrandID))]
        //public Brand? Brand { get; set; }

        [ForeignKey(nameof(CategoryID))]
        public Category? Category { get; set; }

        public ICollection<ProductSizePrice>? ProductSizePrices { get; set; }
    }
}