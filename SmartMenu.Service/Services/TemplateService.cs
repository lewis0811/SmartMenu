using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public TemplateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Template Add(TemplateCreateDTO templateCreateDTO)
        {
            var brand = _unitOfWork.BrandRepository.Find(c => c.BrandId == templateCreateDTO.BrandId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception ("Brand not found or deleted");

            var data = _mapper.Map<Template>(templateCreateDTO);
            _unitOfWork.TemplateRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int templateId)
        {
            var data = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == templateId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Template not found or deleted");

            data.IsDeleted = true;

            _unitOfWork.TemplateRepository.Update(data);
            _unitOfWork.Save();
        }

        public IEnumerable<Template> GetAll(int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.TemplateRepository.EnableQuery();
            var result = DataQuery(data, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Template>();
        }

        public IEnumerable<Template> GetAllWithLayers(int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.TemplateRepository.EnableQuery();
            data = data.Include(c => c.Layers)!
                .ThenInclude(c => c.Boxes)!
                .ThenInclude(c => c.BoxItems)
                .Include(c => c.Layers)!
                .ThenInclude(c => c.LayerItem);

            var result = DataQuery(data, templateId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Template>();
        }

        public Template Update(int templateId, TemplateUpdateDTO templateUpdateDTO)
        {
            var data = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == templateId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception ("Template not found or deleted");

            _mapper.Map(templateUpdateDTO, data);

            _unitOfWork.TemplateRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        private IEnumerable<Template> DataQuery(IQueryable<Template> data, int? templateId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(c => c.IsDeleted == false);
            if (templateId != null)
            {
                data = data
                    .Where(c => c.TemplateId == templateId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                data = data
                    .Where(c => c.TemplateName.Contains(searchString)
                    || c.TemplateDescription!.Contains(searchString));
            }

            return PaginatedList<Template>.Create(data, pageNumber, pageSize);
        }
    }
}