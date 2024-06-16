using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Display : BaseModel
    {
        public int DisplayID { get; set; }
        public int StoreDeviceID { get; set; }
        public int? MenuID { get; set; }
        public int? CollectionID { get; set; }
        public int TemplateID { get; set; }
        public double StartingHour { get; set; }
        public double? EndingHour { get; set; } // optional

        [ForeignKey("StoreDeviceID")]
        public StoreDevice? StoreDevice { get; set; }

        [ForeignKey("MenuID")]
        public Menu? Menu { get; set; }

        [ForeignKey("CollectionID")]
        public Collection? Collection { get; set; }

        [ForeignKey("TemplateID")]
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