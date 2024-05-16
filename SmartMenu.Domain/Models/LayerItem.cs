using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class LayerItem : BaseModel
    {
        public int LayerItemID { get; set; }
        public int LayerID { get; set; }
        public LayerItemType LayerItemType { get; set; }
        public string LayerItemValue { get; set; } = string.Empty;

        //[ForeignKey("LayerID")]
        //public Layer? Layer { get; set; }
    }
}