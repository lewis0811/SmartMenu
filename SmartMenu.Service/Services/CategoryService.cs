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
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Category Add(CategoryCreateDTO categoryCreateDTO)
        {

            var data = _mapper.Map<Category>(categoryCreateDTO);

            _unitOfWork.CategoryRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int categoryId)
        {
            var data = _unitOfWork.CategoryRepository.Find(c => c.CategoryId == categoryId && c.IsDeleted == false).FirstOrDefault()
           ?? throw new Exception("Store not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.CategoryRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Category> GetAll(int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.CategoryRepository.EnableQuery();
            var result = DataQuery(data, categoryId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Category>();
        }

        public Category Update(int categoryId, CategoryCreateDTO categoryCreateDTO)
        {
            var data = _unitOfWork.CategoryRepository.Find(c => c.CategoryId == categoryId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Store not found or deleted");

            _mapper.Map(categoryCreateDTO, data);
            _unitOfWork.CategoryRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }
        private IEnumerable<Category> DataQuery(IQueryable<Category> data, int? categoryId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (categoryId != null)
            {
                data = data
                    .Where(c => c.CategoryId == categoryId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.CategoryName.Contains(searchString)
);
            }

            return PaginatedList<Category>.Create(data, pageNumber, pageSize);
        }
    }
}
