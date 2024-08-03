using SmartMenu.Domain.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace SmartMenu.Domain.Models.DTO
{
    public class LayerCreateDTO
    {
        public int TemplateId { get; set; }

        public LayerType LayerType { get; set; }
    }
}
