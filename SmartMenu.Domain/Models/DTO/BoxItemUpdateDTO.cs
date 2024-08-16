using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxItemUpdateDTO
    {
        [Required(ErrorMessage = "BFont Id is required.")]
        public int BFontId { get; set; }

        public float BoxItemX { get; set; }


        public float BoxItemY { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "Box width must be greater than 0.")]
        public float BoxItemWidth { get; set; }

        [Range(1, float.MaxValue, ErrorMessage = "Box height must be greater than 0.")]
        public float BoxItemHeight { get; set; }

        [Required(ErrorMessage = "Box type is required.")]
        public BoxItemType BoxItemType { get; set; }

        public string? Style { get; set; }
    }
}