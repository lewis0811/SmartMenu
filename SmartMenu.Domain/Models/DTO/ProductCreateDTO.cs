using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductCreateDTO
    {
        public int CategoryId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string ProductDescription { get; set; } = string.Empty;

        [Url]
        public string? ProductImgPath { get; private set; }

        [Url]
        public string? ProductLogoPath { get; private set; }
    }
}