using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Display : BaseModel
    {
        public int DisplayId { get; set; }
        public int StoreDeviceId { get; set; }
        public int? MenuId { get; set; }
        public int? CollectionId { get; set; }
        public int TemplateId { get; set; }
        public double ActiveHour { get; set; }
        public string? DisplayImgPath { get; set; }

        [ForeignKey(nameof(StoreDeviceId))]
        public StoreDevice? StoreDevice { get; set; } //

        [ForeignKey(nameof(MenuId))]
        public Menu? Menu { get; set; }

        [ForeignKey(nameof(CollectionId))]
        public Collection? Collection { get; set; }

        [ForeignKey(nameof(TemplateId))]
        public Template? Template { get; set; }

        public ICollection<DisplayItem>? DisplayItems { get; set; }
    }
}
