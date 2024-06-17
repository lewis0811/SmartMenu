using SmartMenu.Domain.Models.Enum;
using System.Drawing;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxItemUpdateDTO
    {
        public int FontId { get; set; }
        public double FontSize { get; set; }
        public StringAlignment TextFormat { get; set; }
        public BoxType BoxType { get; set; }
        public string BoxColor { get; set; } = "#ffffff";
    }
}