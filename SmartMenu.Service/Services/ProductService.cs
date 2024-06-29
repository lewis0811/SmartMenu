using AutoMapper;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Product Add(ProductCreateDTO productCreateDTO)
        {
            var data = _mapper.Map<Product>(productCreateDTO);

            _unitOfWork.ProductRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int productId)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Product not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Product> GetAll(int? productId, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.ProductRepository.EnableQuery();
            var result = DataQuery(data, productId, categoryId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Product>();
        }

        public Product Update(int productId, ProductUpdateDTO productUpdateDTO)
        {
            var data = _unitOfWork.ProductRepository.Find(c => c.ProductId == productId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Product not found or deleted");

            _mapper.Map(productUpdateDTO, data);
            _unitOfWork.ProductRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<Product> DataQuery(IQueryable<Product> data, int? productId, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (productId != null)
            {
                data = data
                    .Where(c => c.ProductId == productId);
            }

            if (categoryId != null)
            {
                data = data
                    .Where(c => c.CategoryId == categoryId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.ProductName.Contains(searchString)
                    || c.ProductDescription!.Contains(searchString));
            }

            return PaginatedList<Product>.Create(data, pageNumber, pageSize);
        }
    }
}
