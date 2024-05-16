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
        }

        public IBrandRepository BrandRepository { get; private set; }

        public IBrandStaffRepository BrandStaffRepository { get; private set; }

        public IUserRepository UserRepository { get; private set; }

        public IStoreRepository StoreRepository { get; private set; }

        public IRoleRepository RoleRepository { get; private set; }


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