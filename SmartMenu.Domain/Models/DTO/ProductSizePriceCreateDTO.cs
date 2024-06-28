using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductSizePriceCreateDTO
    {
        public int ProductId { get; set; }
        [Range(0, int.MaxValue)]
        public ProductSizeType ProductSizeType { get; set; }
        public double Price { get; set; }
    }
}