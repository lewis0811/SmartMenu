using AutoMapper;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.API.Ultility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Store, StoreCreateDTO>().ReverseMap();
            CreateMap<Store, StoreUpdateDTO>().ReverseMap();
            CreateMap<Template, TemplateCreateDTO>().ReverseMap();
            CreateMap<Template, TemplateUpdateDTO>().ReverseMap();
            CreateMap<Layer, LayerCreateDTO>().ReverseMap();
            CreateMap<Layer, LayerUpdateDTO>().ReverseMap();
            CreateMap<Box, BoxCreateDTO>().ReverseMap();
            CreateMap<Box, BoxUpdateDTO>().ReverseMap();
            CreateMap<Brand, BrandCreateDTO>().ReverseMap();
            CreateMap<Brand, BrandUpdateDTO>().ReverseMap();
            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Product, ProductCreateDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap();
            CreateMap<Collection, CollectionCreateDTO>().ReverseMap();
            CreateMap<Collection, CollectionUpdateDTO>().ReverseMap();
            CreateMap<Menu, MenuCreateDTO>().ReverseMap();
            CreateMap<Menu, MenuUpdateDTO>().ReverseMap();
            CreateMap<StoreMenu, StoreMenuCreateDTO>().ReverseMap();
            CreateMap<ProductGroup, ProductGroupCreateDTO>().ReverseMap();
            CreateMap<ProductGroup, ProductGroupUpdateDTO>().ReverseMap();
            CreateMap<ProductGroupItem, ProductGroupItemCreateDTO>().ReverseMap();
            CreateMap<StoreCollection, StoreCollectionCreateDTO>().ReverseMap();
            CreateMap<BrandStaff, BrandStaffCreateDTO>().ReverseMap();
            CreateMap<User, UserCreateDTO>().ReverseMap();
            CreateMap<User, UserLoginDTO>().ReverseMap();
            CreateMap<User, UserUpdateDTO>().ReverseMap();
            CreateMap<Font, FontCreateDTO>().ReverseMap();
            CreateMap<StoreProduct, StoreProductCreateDTO>().ReverseMap();
            CreateMap<StoreProduct, StoreProductUpdateDTO>().ReverseMap();
            CreateMap<StoreDevice, StoreDeviceCreateDTO>().ReverseMap();
            CreateMap<StoreDevice, StoreDeviceUpdateDTO>().ReverseMap();
            CreateMap<BoxItem, BoxItemCreateDTO>().ReverseMap();
            CreateMap<BoxItem, BoxItemUpdateDTO>().ReverseMap();
            CreateMap<ProductSize, ProductSizeCreateDTO>().ReverseMap();
            CreateMap<ProductSizePrice, ProductSizePriceUpdateDTO>().ReverseMap();
            CreateMap<ProductSizePrice, ProductSizePriceCreateDTO>().ReverseMap();
            CreateMap<LayerItem, LayerItemCreateDTO>().ReverseMap();
            CreateMap<LayerItem, LayerItemUpdateDTO>().ReverseMap();
            CreateMap<Display, DisplayCreateDTO>().ReverseMap();
            CreateMap<DisplayItem, DisplayItemUpdateDTO>().ReverseMap();
            CreateMap<DisplayItem, DisplayItemCreateDTO>().ReverseMap();
        }
    }
}