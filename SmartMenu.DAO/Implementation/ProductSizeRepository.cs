//using SmartMenu.Domain.Models;
//using SmartMenu.Domain.Repository;

//namespace SmartMenu.DAO.Implementation
//{
//    public class ProductSizeRepository : GenericRepository<ProductSize>, IProductSizeRepository
//    {
//        private readonly SmartMenuDBContext _context;

//        public ProductSizeRepository(SmartMenuDBContext context) : base(context)
//        {
//            _context = context;
//        }

//        public IEnumerable<ProductSize> GetAll(int? productSizeId, string? searchString, int pageNumber, int pageSize)
//        {
//            var data = _context.ProductSizes.AsQueryable();
//            return DataQuery(data, productSizeId, searchString, pageNumber, pageSize);
//        }

//        private IEnumerable<ProductSize> DataQuery(IQueryable<ProductSize> data, int? productSizeId, string? searchString, int pageNumber, int pageSize)
//        {
//            data = data.Where(c => c.IsDeleted == false);
//            if (productSizeId != null)
//            {
//                data = data
//                    .Where(c => c.ProductSizeId == productSizeId);
//            }
//            if (searchString != null)
//            {
//                searchString = searchString.Trim();
//                data = data
//                    .Where(c => c.SizeName.Contains(searchString));
//            }
//            return PaginatedList<ProductSize>.Create(data, pageNumber, pageSize);
//        }
//    }
//}