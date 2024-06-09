using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace SmartMenu.Domain.Models
{
    public class BoxContent
    {
        public int BoxContentID { get; set; }
        public int FontID { get; set; }
        public float FontSize { get; set; }
        public StringAlignment FontAlignment { get; set; }
        public BoxContentType ContentType { get; set; }
        public string BoxColor { get; set; } = "#ffffff";

        [ForeignKey(nameof(FontID))]
        public Font? Font { get; set; }
    }
}
