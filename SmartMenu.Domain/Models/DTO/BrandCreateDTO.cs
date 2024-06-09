using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandCreateDTO
    {
        [Required]
        public string BrandName { get; set; } = string.Empty;

        public string BrandDescription { get; set; } = string.Empty;
        public string BrandImage { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string BrandContactEmail { get; set; } = string.Empty;
    }
}
