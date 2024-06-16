namespace SmartMenu.Domain.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IBrandRepository BrandRepository { get; }
        IBrandStaffRepository BrandStaffRepository { get; }
        IUserRepository UserRepository { get; }
        IStoreRepository StoreRepository { get; }
        IRoleRepository RoleRepository { get; }
        ITemplateRepository TemplateRepository { get; }
        ILayerRepository LayerRepository { get; }
        ILayerItemRepository LayerItemRepository { get; }
        IBoxRepository BoxRepository { get; }
        IStoreDeviceRepository StoreDeviceRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        ICollectionRepository CollectionRepository { get; }
        IMenuRepository MenuRepository { get; }
        IStoreMenuRepository StoreMenuRepository { get; }
        IProductGroupRepository ProductGroupRepository { get; }
        IProductGroupItemRepository ProductGroupItemRepository { get; }
        IStoreCollectionRepository StoreCollectionRepository { get; }

        IFontRepository FontRepository { get; }
        IStoreProductRepository StoreProductRepository { get; }
        int Save();
    }
}