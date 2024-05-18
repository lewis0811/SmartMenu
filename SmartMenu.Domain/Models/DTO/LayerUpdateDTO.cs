using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerUpdateDTO
    {
        [Required]
        public string LayerName { get; set; } = string.Empty;
    }
}
