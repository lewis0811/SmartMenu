using System.ComponentModel.DataAnnotations.Schema;

namespace SmartMenu.Domain.Models
{
    public class StoreMenu : BaseModel
    {
        public int StoreMenuId { get; set; }
        public int StoreId { get; set; }
        public int MenuId { get; set; }

        [ForeignKey(nameof(MenuId))]
        public Menu? Menu { get; set; }

        //[ForeignKey(nameof(StoreId))]
        //public Store? Store { get; set; } //
    }
}