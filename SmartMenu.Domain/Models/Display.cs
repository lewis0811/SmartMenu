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
        public double StartingHour { get; set; }
        public double? EndingHour { get; set; } // optional
        public string? DisplayImgPath { get; set; } 

        [ForeignKey("StoreDeviceId")]
        public StoreDevice? StoreDevice { get; set; }

        [ForeignKey("MenuId")]
        public Menu? Menu { get; set; }

        [ForeignKey("CollectionId")]
        public Collection? Collection { get; set; }

        [ForeignKey("TemplateId")]
        public Template? Template { get; set; }

        public ICollection<DisplayItem>? DisplayItems { get; set; }
    }
}

/*
 * var currentTime = DateTime.Now.Hour + (DateTime.Now.Minute / 60.0); // Current time in fractional hours
var activeDisplay = dbContext.Displays
    .Where(d => d.StartingHour <= currentTime && (d.EndingHour == null || d.EndingHour >= currentTime)) // Change > to >=
    .OrderByDescending(d => d.StartingHour) // Get the latest one if multiple match
    .FirstOrDefault();
*/