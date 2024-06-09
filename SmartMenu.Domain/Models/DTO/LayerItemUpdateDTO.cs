using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerItemUpdateDTO
    {
        public LayerType LayerItemType { get; set; }
        [Required]
        public string LayerItemValue { get; set; } = string.Empty;
    }
}
