using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxCreateDTO
    {
        [Required(ErrorMessage = "Layer ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Layer ID must be a positive integer.")]
        public int LayerId { get; set; }

        public float BoxPositionX { get; set; }

        public float BoxPositionY { get; set; }

        [Required(ErrorMessage = "Box width is required.")]
        [Range(1, float.MaxValue, ErrorMessage = "Box width must be greater than 0.")] // Adjusted range to avoid 0 width
        public float BoxWidth { get; set; }

        [Required(ErrorMessage = "Box height is required.")]
        [Range(1, float.MaxValue, ErrorMessage = "Box height must be greater than 0.")] // Adjusted range to avoid 0 height
        public float BoxHeight { get; set; }

        public BoxType BoxType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Box max capacity must be at least 1.")] // This validation was already present
        public int BoxMaxCapacity { get; set; } = 1;
    }
}