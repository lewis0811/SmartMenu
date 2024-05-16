using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreMenu : BaseModel
    {
        public int StoreMenuID { get; set; }
        public int StoreID { get; set; }
        public int MenuID { get; set; }

        [ForeignKey("MenuID")]
        public Menu? Menu { get; set; }

        //[ForeignKey(nameof(StoreID))]
        //public Store? Store { get; set; }
    }
}