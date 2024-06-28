using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class LayerItem : BaseModel
    {
        public int LayerItemId { get; set; }

        [Required]
        public string LayerItemValue { get; set; } = string.Empty;
    }
}