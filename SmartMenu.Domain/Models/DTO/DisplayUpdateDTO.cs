using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class DisplayUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Menu ID must be a positive integer.")] // Only validate if not null
        public int? MenuId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Collection ID must be a positive integer.")] // Only validate if not null
        public int? CollectionId { get; set; }

        [Required(ErrorMessage = "Template ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Template ID must be a positive integer.")]
        public int TemplateId { get; set; }

        [Required(ErrorMessage = "Active hour is required.")]
        [Range(0, 23.99, ErrorMessage = "Active hour must be between 0 and 23.99.")]
        public double ActiveHour { get; set; }

        [Required(ErrorMessage = "Display image path is required.")]
        [Url(ErrorMessage = "Display image path must be a valid URL.")] // Use UrlAttribute for URL validation
        public string DisplayImgPath { get; set; } = string.Empty;
    }
}
