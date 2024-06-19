using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxItemUpdateDTO
    {
        [Required(ErrorMessage = "Font ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Font ID must be a positive integer.")]
        public int FontId { get; set; }

        [Required(ErrorMessage = "Font size is required.")]
        [Range(8, 400, ErrorMessage = "Font size must be between 8 and 400.")] // Adjust the range as needed
        public double FontSize { get; set; }

        [Required(ErrorMessage = "Text format is required.")]
        public StringAlignment TextFormat { get; set; }

        [Required(ErrorMessage = "Box type is required.")]
        public BoxType BoxType { get; set; }

        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Invalid box color. Use a valid hexadecimal color code (e.g., #RRGGBB).")]
        public string BoxColor { get; set; } = "#ffffff";
    }
}