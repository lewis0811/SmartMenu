﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmartMenu.DAO;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using SmartMenu.Domain.Models.Enum;
using SmartMenu.Domain.Repository;
using SmartMenu.Service.Interfaces;

namespace SmartMenu.Service.Services
{
    public class BoxService : IBoxService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private Domain.Models.Layer tempLayer = new();

        public BoxService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Box> GetAll(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BoxRepository.EnableQuery();
            var result = DataQuery(data, boxId, layerId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Box>();
        }

        public IEnumerable<Box> GetAllWithBoxItems(int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            var data = _unitOfWork.BoxRepository.EnableQuery();
            data = data.Include(c => c.BoxItems);

            var result = DataQuery(data, boxId, layerId, searchString, pageNumber, pageSize);

            return result ?? Enumerable.Empty<Box>();
        }

        private static IEnumerable<Box> DataQuery(IQueryable<Box> data, int? boxId, int? layerId, string? searchString, int pageNumber, int pageSize)
        {
            data = data.Where(data => data.IsDeleted == false);

            if (boxId != null)
            {
                data = data
                    .Where(c => c.BoxId == boxId);
            }

            if (layerId != null)
            {
                data = data
                    .Where(c => c.LayerId == layerId);
            }

            if (searchString != null)
            {
                searchString = searchString.Trim();
                if (int.TryParse(searchString, out int searchNumber)) // Try to parse the search string to an integer
                {
                    data = data.Where(c =>
                        c.MaxProductItem == searchNumber
                        || c.BoxPositionX == searchNumber
                        || c.BoxPositionY == searchNumber);
                }
            }

            return PaginatedList<Box>.Create(data, pageNumber, pageSize);
        }

        public Box Add(BoxCreateDTO boxCreateDTO)
        {
            var layer = _unitOfWork.LayerRepository
                .Find(c => c.LayerId == boxCreateDTO.LayerId && c.IsDeleted == false)
                .FirstOrDefault()
                ?? throw new Exception("Layer not found or deleted");

            var data = _mapper.Map<Box>(boxCreateDTO);
            if (layer.LayerType == LayerType.Content) { data.BoxType = BoxType.UseInDisplay; }
            else
            {
                var existBox = _unitOfWork.BoxRepository.EnableQuery().Any(x => x.LayerId == data.LayerId);
                if (existBox) throw new Exception("Box already exists in layer");
                data.BoxType = BoxType.UseInTemplate;
                data.MaxProductItem = 0;
            }

            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == layer.TemplateId && c.IsDeleted == false).FirstOrDefault()!;
            if (data.BoxPositionX > template.TemplateWidth || data.BoxPositionY > template.TemplateHeight)
            {
                throw new Exception("Box position must be in template resolution");
            }

            _unitOfWork.BoxRepository.Add(data);
            _unitOfWork.Save();

            return data;
        }

        public Box Update(int boxId, BoxUpdateDTO boxUpdateDTO)
        {
            var data = _unitOfWork.BoxRepository.Find(c => c.BoxId == boxId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Box not found or deleted");

            tempLayer = _unitOfWork.LayerRepository.Find(c => c.LayerId == data.LayerId && c.IsDeleted == false).FirstOrDefault()
                ?? throw new Exception("Layer not found or deleted");

            var template = _unitOfWork.TemplateRepository.Find(c => c.TemplateId == tempLayer.TemplateId && c.IsDeleted == false).FirstOrDefault()!;
            if (data.BoxPositionX > template.TemplateWidth || data.BoxPositionY > template.TemplateHeight)
            {
                throw new Exception("Box position must be in template resolution");
            }

            _mapper.Map(boxUpdateDTO, data);

            _unitOfWork.BoxRepository.Update(data);
            _unitOfWork.Save();

            return data;
        }

        public void Delete(int boxId)
        {
            var data = _unitOfWork.BoxRepository.Find(c => c.BoxId == boxId && c.IsDeleted == false).FirstOrDefault()
            ?? throw new Exception("Box not found or deleted");

            data.IsDeleted = true;
            _unitOfWork.BoxRepository.Update(data);
            _unitOfWork.Save();
        }
    }
}