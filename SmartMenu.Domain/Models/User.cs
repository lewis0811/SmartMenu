using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class User : BaseModel
    {
        public Guid UserId { get; set; } = new Guid();

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public Guid? Token { get; set; }

        public Role Role { get; set; }
    }
}