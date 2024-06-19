using System.ComponentModel.DataAnnotations;
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
        [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than or equal to 1.")]
        public int BoxMaxCapacity { get; set; } = 1;

        //[ForeignKey(nameof(LayerID))]
        //public Layer? Layer { get; set; }

        public ICollection<BoxItem>? BoxItems { get; set; }
    }
}