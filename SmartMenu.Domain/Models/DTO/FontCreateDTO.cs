using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class FontCreateDTO
    {
        [Required]
        public IFormFile? File { get; set; }
    }
}
