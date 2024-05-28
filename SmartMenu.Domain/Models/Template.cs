﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Template : BaseModel
    {
        public int TemplateID { get; set; }
        public int? BrandID { get; set; }

        [Required]
        public string TemplateName { get; set; } = string.Empty;

        public string TemplateDescription { get; set; } = string.Empty;

        [ForeignKey(nameof(BrandID))]
        public Brand? Brand { get; set; }

        public ICollection<Layer>? Layers { get; set; }
    }
}