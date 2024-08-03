using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxItemUpdateDTO
    {
        [Required(ErrorMessage = "BFont Id is required.")]
        public int BFontId { get; set; }

        public float BoxItemX { get; set; }

        public float BoxItemY { get; set; }
        public float BoxItemWidth { get; set; }
        public float BoxItemHeight { get; set; }

        [Required(ErrorMessage = "Box item type is required.")]
        public BoxItemType BoxItemType { get; set; }

        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid box color. Use a valid hexadecimal color code (e.g., #RRGGBB).")]
        public string BoxColor { get; set; } = "#ffffff";
    }
}