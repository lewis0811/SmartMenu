using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models
{
    public class Role : BaseModel
    {
        public Guid RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;
    }
}