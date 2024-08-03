using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxItemCreateDTO
    {
        [Required(ErrorMessage = "Box ID is required.")]
        public int BoxId { get; set; }

        [Required(ErrorMessage = "BFont Id is required.")]
        public int BFontId { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Box position X must be a non-negative number.")]
        public float BoxItemX { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Box position Y must be a non-negative number.")]
        public float BoxItemY { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "Box width must be greater than 0.")]
        public float BoxItemWidth { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "Box height must be greater than 0.")]
        public float BoxItemHeight { get; set; }

        [Required(ErrorMessage = "Box type is required.")]
        public BoxItemType BoxItemType { get; set; }
    }
}