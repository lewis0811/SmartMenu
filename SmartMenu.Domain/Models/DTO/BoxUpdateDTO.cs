using SmartMenu.Domain.Models.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class BoxUpdateDTO
    {
        public int FontID { get; set; }
        public string? BoxContent { get; set; } = string.Empty;

        public double FontSize { get; set; }

        [Required]
        public BoxType BoxType { get; set; }

        [Required]
        public string BoxColor { get; set; } = "#ffffff";

        public double BoxPositionX { get; set; }
        public double BoxPositionY { get; set; }
        public int BoxMaxCapacity { get; set; }
    }
}
