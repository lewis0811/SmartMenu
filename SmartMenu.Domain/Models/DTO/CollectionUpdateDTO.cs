﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Domain.Models.DTO
{
    public class CollectionUpdateDTO
    {

        [Required]
        public string CollectionName { get; set; } = string.Empty;

        public string CollectionDescription { get; set; } = string.Empty;
    }
}
