using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandStaffCreateDTO
    {
        public int BrandID { get; set; }
        [Required]
        public Guid UserID { get; set; }
    }
}
