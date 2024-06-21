using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class DisplayCreateDTO
    {

        [Required(ErrorMessage = "Store Device ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Store Device ID must be a positive integer.")]
        public int StoreDeviceId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Menu ID must be a positive integer.")] // Only validate if not null
        public int? MenuId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Collection ID must be a positive integer.")] // Only validate if not null
        public int? CollectionId { get; set; }

        [Required(ErrorMessage = "Template ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Template ID must be a positive integer.")]
        public int TemplateId { get; set; }

        [Required(ErrorMessage = "Starting hour is required.")]
        [Range(0, 23.99, ErrorMessage = "Starting hour must be between 0 and 23.99.")]
        public double StartingHour { get; set; }

        [Range(0, 23.99, ErrorMessage = "Ending hour must be between 0 and 23.99.")] // Only validate if not null
        public double? EndingHour { get; set; }

        [Required(ErrorMessage = "Display image path is required.")]
        [Url(ErrorMessage = "Display image path must be a valid URL.")] // Use UrlAttribute for URL validation
        public string DisplayImgPath { get; set; } = string.Empty;
    }
}