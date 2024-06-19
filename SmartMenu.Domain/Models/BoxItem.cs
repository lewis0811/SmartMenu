using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace SmartMenu.Domain.Models
{
    public class BoxItem : BaseModel
    {
        public int BoxItemId { get; set; }
        public int BoxId { get; set; }
        public int FontId { get; set; }
        public double FontSize { get; set; }
        public StringAlignment TextFormat { get; set; }
        public BoxType BoxType { get; set; }
        public string BoxColor { get; set; } = "#ffffff";

        //[ForeignKey(nameof(BoxId))]
        //public Box? Box { get; set; }

        [ForeignKey(nameof(FontId))]
        public Font? Font { get; set; }
    }
}