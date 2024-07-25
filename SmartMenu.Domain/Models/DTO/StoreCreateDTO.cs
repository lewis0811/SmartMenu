using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class StoreCreateDTO
    {
        public int BrandID { get; set; }

        [Required]
        public string? StoreName { get; set; }
        [Required]
        public string StoreLocation { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string StoreContactEmail { get; set; } = string.Empty;

        [Phone]
        [Required]
        public string StoreContactNumber { get; set; } = string.Empty;
        public bool StoreStatus { get; set; }
    }
}