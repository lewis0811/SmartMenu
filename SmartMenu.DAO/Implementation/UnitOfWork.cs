using SmartMenu.Domain.Repository;

namespace SmartMenu.DAO.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SmartMenuDBContext _context;

        public UnitOfWork(SmartMenuDBContext context)
        {
            _context = context;
            BrandRepository = new BrandRepository(_context);
            BrandStaffRepository = new BrandStaffRepository(_context);
            UserRepository = new UserRepository(_context);
            StoreRepository = new StoreRepository(_context);
            RoleRepository = new RoleRepository(_context);
            TemplateRepository = new TemplateRepository(_context);
            LayerRepository = new LayerRepository(_context);
            LayerItemRepository = new LayerItemRepository(_context);
            BoxRepository = new BoxRepository(_context);
            StoreDeviceRepository = new StoreDeviceRepository(_context);
        }

        public IBrandRepository BrandRepository { get; private set; }

        public IBrandStaffRepository BrandStaffRepository { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IStoreRepository StoreRepository { get; private set; }

        public IRoleRepository RoleRepository { get; private set; }

        public ITemplateRepository TemplateRepository { get; private set; }
        public ILayerRepository LayerRepository { get; private set; }
        public ILayerItemRepository LayerItemRepository { get; private set; }
        public IBoxRepository BoxRepository { get; private set; }
        public IStoreDeviceRepository StoreDeviceRepository { get; private set; }
        public void Dispose()
         {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}