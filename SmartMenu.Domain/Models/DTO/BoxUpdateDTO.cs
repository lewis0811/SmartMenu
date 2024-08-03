using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxUpdateDTO
    {
        [Range(0, float.MaxValue, ErrorMessage = "Box position X must be a non-negative number.")]
        public float BoxPositionX { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Box position Y must be a non-negative number.")]
        public float BoxPositionY { get; set; }

        [Required(ErrorMessage = "Box width is required.")]
        [Range(1, float.MaxValue, ErrorMessage = "Box width must be greater than 0.")] 
        public float BoxWidth { get; set; }

        [Required(ErrorMessage = "Box height is required.")]
        [Range(1, float.MaxValue, ErrorMessage = "Box height must be greater than 0.")] 
        public float BoxHeight { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Value must be greater than or equal to 0.")]
        public int MaxProductItem { get; set; }
    }
}
