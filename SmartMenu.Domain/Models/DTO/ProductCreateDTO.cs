﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class ProductCreateDTO
    {
        public int BrandID { get; set; }
        public int CategoryID { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public string ProductDescription { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public double ProductPrice { get; set; } = 1;
    }
}
