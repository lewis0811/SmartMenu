using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductSizePriceCreateDTO
    {
        public int ProductId { get; set; }
        [Range(0, int.MaxValue)]
        public ProductSizeType ProductSizeType { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public double Price { get; set; }
    }
}