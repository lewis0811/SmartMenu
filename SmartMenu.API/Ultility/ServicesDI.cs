using CloudinaryDotNet;
using SmartMenu.DAO.Implementation;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using SmartMenu.Service.Services;

namespace SmartMenu.API.Ultility
{
    public static class ServicesDI
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services)
        {
            // Repositories DI
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services DI
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBoxService, BoxService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddScoped<IBoxItemService, BoxItemService>();
            services.AddScoped<ILayerService, LayerService>();
            services.AddScoped<IlayerItemService, LayerItemService>();
            services.AddScoped<IDisplayService, DisplayService>();
            services.AddScoped<IDisplayItemService, DisplayItemService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IStoreCollectionService, StoreCollectionService>();
            services.AddScoped<IStoreMenuService, StoreMenuService>();
            services.AddScoped<IStoreProductService, StoreProductService>();
            services.AddScoped<IStoreDeviceService, StoreDeviceService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICollectionService, CollectionService>();
            services.AddScoped<IBrandStaffService, BrandStaffService>();
            services.AddScoped<IProductSizePriceService, ProductSizePriceService>();
            services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<IProductSizeService, ProductSizeService>();
            services.AddScoped<IProductGroupItemService, ProductGroupItemService>();
            services.AddScoped<IProductGroupService, ProductGroupService>();
            services.AddScoped<IFontService, FontService>();
            services.AddScoped<ICloudinary, Cloudinary>();
            services.AddScoped<IEnumService, EnumService>();
            return services;
        }
    }
}