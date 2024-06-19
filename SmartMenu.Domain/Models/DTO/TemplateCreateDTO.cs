﻿using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class TemplateCreateDTO
    {
        [Required(ErrorMessage = "Brand ID is required.")]
        public int BrandID { get; set; }

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
