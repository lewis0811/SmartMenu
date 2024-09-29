using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductUpdateDTO
    {
        public int CategoryId { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string ProductDescription { get; set; } = string.Empty;

        public ProductPriceCurrency ProductPriceCurrency { get; set; }

        [Url]
        public string? ProductImgPath { get; set; }

        //[Url]
        //public string? ProductLogoPath { get;  set; }
    }
}
