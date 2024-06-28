using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerItemCreateDTO
    {
        public int LayerID { get; set; }
        [Required]
        public string LayerItemValue { get; set; } = string.Empty;
    }
}