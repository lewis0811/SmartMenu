using AutoMapper;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartMenu.Service.Interfaces
{
    public interface ITemplateService
    {
        IEnumerable<Template> GetAll(int? templateId, int? brandId, string? searchString, int pageNumber, int pageSize);
        IEnumerable<Template> GetAllWithLayers(int? templateId, int? brandId, string? searchString, int pageNumber, int pageSize);
        Template Add(TemplateCreateDTO templateCreateDTO);
        Template Update(int templateId, TemplateUpdateDTO templateUpdateDTO);
        void Delete(int templateId);
        object Update(int templateId, string templateImgPath);
    }
}
