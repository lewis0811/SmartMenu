using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class TemplateCreateDTO
    {
        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string TemplateDescription { get; set; } = string.Empty;
    }
}
