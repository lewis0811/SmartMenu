using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class DisplayItemCreateDTO
    {
        [Required(ErrorMessage = "Display ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Display ID must be a positive integer.")]
        public int DisplayID { get; set; }

        [Required(ErrorMessage = "Box ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Box ID must be a positive integer.")]
        public int BoxID { get; set; }

        [Required(ErrorMessage = "Product Group ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Product Group ID must be a positive integer.")]
        public int ProductGroupID { get; set; }
    }
}
