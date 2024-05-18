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
        int Save();
    }
}