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
        }
    }
}
