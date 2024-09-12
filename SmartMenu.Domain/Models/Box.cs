using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Box : BaseModel
    {
        public int BoxId { get; set; }
        public int LayerId { get; set; }

        public float BoxPositionX { get; set; }
        public float BoxPositionY { get; set; }
        public float BoxWidth { get; set; }
        public float BoxHeight { get; set; }
        public BoxType BoxType { get; set; }

        //public int MaxProductItem { get; set; }

        //[ForeignKey(nameof(LayerId))] //
        //public Layer? Layer { get; set; }

        public ICollection<BoxItem>? BoxItems { get; set; }
    }
}