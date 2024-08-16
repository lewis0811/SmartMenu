using Microsoft.EntityFrameworkCore;
using SmartMenu.Domain.Models;

namespace SmartMenu.DAO
{
    public class SmartMenuDBContext : DbContext
    {
        public SmartMenuDBContext(DbContextOptions options) : base(options)
        {
            Boxes = Set<Box>();
            Brands = Set<Brand>();
            BrandStaffs = Set<BrandStaff>();
            Categories = Set<Category>();
            Collections = Set<Collection>();
            Displays = Set<Display>();
            DisplayItems = Set<DisplayItem>();
            BFonts = Set<BFont>();
            Layers = Set<Layer>();
            LayersItem = Set<LayerItem>();
            Menus = Set<Menu>();
            Products = Set<Product>();
            ProductGroups = Set<ProductGroup>();
            ProductGroupsItem = Set<ProductGroupItem>();
            //Schedules = Set<Schedule>();
            Stores = Set<Store>();
            StoreCollections = Set<StoreCollection>();
            StoreDevices = Set<StoreDevice>();
            StoreMenus = Set<StoreMenu>();
            StoreProducts = Set<StoreProduct>();
            Templates = Set<Template>();
            Users = Set<User>();
            //ProductSizes = Set<ProductSize>();
            ProductSizePrices = Set<ProductSizePrice>();
            BoxItems = Set<BoxItem>();
            Subscriptions = Set<Subscription>();
            DeviceSubscriptions = Set<DeviceSubscription>();
            Transactions = Set<Transaction>();
        }

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandStaff> BrandStaffs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Display> Displays { get; set; }
        public DbSet<DisplayItem> DisplayItems { get; set; }
        public DbSet<BFont> BFonts { get; set; }
        public DbSet<Layer> Layers { get; set; }
        public DbSet<LayerItem> LayersItem { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ProductGroupItem> ProductGroupsItem { get; set; }
        //public DbSet<Role> Roles { get; set; }
        //public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreCollection> StoreCollections { get; set; }
        public DbSet<StoreDevice> StoreDevices { get; set; }
        public DbSet<StoreMenu> StoreMenus { get; set; }
        public DbSet<StoreProduct> StoreProducts { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<User> Users { get; set; }
        //public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ProductSizePrice> ProductSizePrices { get; set; }
        public DbSet<BoxItem> BoxItems { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<DeviceSubscription> DeviceSubscriptions { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}