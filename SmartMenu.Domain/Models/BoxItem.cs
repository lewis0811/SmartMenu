using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class BoxItem : BaseModel
    {
        public int BoxItemId { get; set; }
        public int BoxId { get; set; }
        public int? BFontId { get; set; } = null;
        public float BoxItemX { get; set; }
        public float BoxItemY { get; set; }
        public float BoxItemWidth { get; set; }
        public float BoxItemHeight { get; set; }
        public BoxItemType BoxItemType { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than or equal to 1.")]
        public int Order { get; set; }

        public string? Style { get; set; }

        [ForeignKey(nameof(BoxId))]
        public Box? Box { get; set; } //

        [ForeignKey(nameof(BFontId))]
        public BFont? BFont { get; set; }
    }
}