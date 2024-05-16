using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Box : BaseModel
    {
        public int BoxID { get; set; }
        public int LayerID { get; set; }
        public int FontID { get; set; }
        public int BoxContentID { get; set; }

        public double FontSize { get; set; }

        [Required]
        public BoxType BoxType { get; set; }

        [Required]
        public string BoxColor { get; set; } = "#ffffff";

        public double BoxPositionX { get; set; }
        public double BoxPositionY { get; set; }
        public int BoxMaxCapacity { get; set; }

        //[ForeignKey(nameof(LayerID))]
        //public Layer? Layer { get; set; }

        [ForeignKey(nameof(FontID))]
        public Font? Font { get; set; }

        [ForeignKey(nameof(BoxContentID))]
        public BoxContent? BoxContent { get; set; }
    }
}