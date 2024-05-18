using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class MenuUpdateDTO
    {

        [Required]
        public string MenuName { get; set; } = string.Empty;

        public string MenuDescription { get; set; } = string.Empty;

    }
}
