using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Box : BaseModel
    {
        public int BoxID { get; set; }
        public int LayerID { get; set; }

        public float BoxPositionX { get; set; }
        public float BoxPositionY { get; set; }
        public float BoxWidth { get; set; }
        public float BoxHeight { get; set; }
        public int BoxMaxCapacity { get; set; }

        [ForeignKey(nameof(LayerID))]
        public Layer? Layer { get; set; }
    }
}