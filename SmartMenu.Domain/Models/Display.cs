using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class Display : BaseModel
    {
        public int DisplayID { get; set; }
        //public int StoreDeviceID { get; set; }
        public int MenuID { get; set; }
        public int CollectionID { get; set; }
        public int TemplateID { get; set; }
        public double StartingHour { get; set; }

        //[ForeignKey("StoreDeviceID")]
        //public StoreDevice? StoreDevice { get; set; }

        [ForeignKey("MenuID")]
        public Menu? Menu { get; set; }

        [ForeignKey("CollectionID")]
        public Collection? Collection { get; set; }

        [ForeignKey("TemplateID")]
        public Template? Template { get; set; }

        //[ForeignKey("ScheduleID")]
        //public Schedule? Schedule { get; set; }

        public ICollection<DisplayItem>? DisplayItems { get; set; }
    }
}