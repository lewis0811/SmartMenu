using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class LayerItem : BaseModel
    {
        public int LayerItemId { get; set; }
        public int LayerId { get; set; }
        [Required]
        public string LayerItemValue { get; set; } = string.Empty;

        //[ForeignKey(nameof(LayerId))]
        //public Layer? Layer { get; set; } //
    }
}