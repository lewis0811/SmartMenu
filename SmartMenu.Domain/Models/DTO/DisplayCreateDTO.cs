using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class DisplayCreateDTO
    {

        [Required(ErrorMessage = "Store Device ID is required.")]
        public int StoreDeviceId { get; set; }

        public int? MenuId { get; set; }

        public int? CollectionId { get; set; }

        [Required(ErrorMessage = "Template ID is required.")]
        public int TemplateId { get; set; }

        [Required(ErrorMessage = "Active hour is required.")]
        [Range(0, 23.99, ErrorMessage = "Active hour must be between 0 and 23.99.")]
        public double ActiveHour { get; set; }

        //[Url(ErrorMessage = "Display image path must be a valid URL.")] // Use UrlAttribute for URL validation
        public string? DisplayImgPath { get; set; }
    }
}