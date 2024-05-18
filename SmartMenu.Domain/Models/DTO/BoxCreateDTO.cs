using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxCreateDTO
    {
        public int LayerID { get; set; }
        public int FontID { get; set; }
        public string? BoxContent { get; set; } = string.Empty;

        public double FontSize { get; set; }

        [Required]
        public BoxType BoxType { get; set; }

        [Required]
        public string BoxColor { get; set; } = "#ffffff";

        public double BoxPositionX { get; set; }
        public double BoxPositionY { get; set; }
        public int BoxMaxCapacity { get; set; }
    }
}