using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandCreateDTO
    {
        [Required]
        public string BrandName { get; set; } = string.Empty;

        public string BrandDescription { get; set; } = string.Empty;
        public string BrandImage { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string BrandContactEmail { get; set; } = string.Empty;
    }
}
