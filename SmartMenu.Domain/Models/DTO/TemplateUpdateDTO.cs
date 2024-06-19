using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class TemplateUpdateDTO
    {
        [Required(ErrorMessage = "Template Name is required.")]
        [StringLength(200, ErrorMessage = "Template Name cannot exceed 200 characters.")] // Example length constraint
        public string TemplateName { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Template Description cannot exceed 255 characters.")] // Example length constraint
        public string TemplateDescription { get; set; } = string.Empty;

        [Range(40f, 8000f, ErrorMessage = "Template Width must be between 40 and 8000f.")] // Example range constraint
        public float TemplateWidth { get; set; }

        [Range(40f, 3125f, ErrorMessage = "Template Height must be between 40 and 3125f.")] // Example range constraint
        public float TemplateHeight { get; set; }
    }
}
