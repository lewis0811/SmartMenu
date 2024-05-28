    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class User  : BaseModel
    {
        public Guid UserID { get; set; } = new Guid();

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public Guid RoleID { get; set; }

        [ForeignKey("RoleID")]
        public Role? Role { get; set; }
    }
}