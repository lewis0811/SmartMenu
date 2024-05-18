using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductGroupUpdateDTO
    {
        [Required]
        public string ProductGroupName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int ProductGroupMaxCapacity { get; set; }
    }
}
