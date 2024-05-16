using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models
{
    public class Font : BaseModel
    {
        public int FontID { get; set; }
        [Required]
        public string FontName { get; set; } = string.Empty;
        [Required]
        public string FontPath { get; set; } = string.Empty;


    }
}
