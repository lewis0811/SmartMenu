using SmartMenu.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerItemUpdateDTO
    {
        public LayerItemType LayerItemType { get; set; }
        [Required]
        public string LayerItemValue { get; set; } = string.Empty;
    }
}
