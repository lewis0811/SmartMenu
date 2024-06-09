using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class StoreUpdateDTO
    {
        [Required]
        public string StoreLocation { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string StoreContactEmail { get; set; } = string.Empty;

        [Phone]
        [Required]
        public string StoreContactNumber { get; set; } = string.Empty;
    }
}
