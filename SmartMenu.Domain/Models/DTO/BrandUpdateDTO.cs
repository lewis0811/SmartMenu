using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandUpdateDTO
    {
        public string BrandDescription { get; set; } = string.Empty;
        public string BrandImage { get; set; } = string.Empty;
    }
}
