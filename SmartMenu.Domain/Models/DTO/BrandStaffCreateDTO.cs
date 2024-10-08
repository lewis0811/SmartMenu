﻿using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class BrandStaffCreateDTO
    {
        public int BrandId { get; set; }
        [Required]
        public Guid UserId { get; set; }

        public int StoreId { get; set; }
    }
}
