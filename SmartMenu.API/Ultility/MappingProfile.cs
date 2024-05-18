﻿using AutoMapper;
using SmartMenu.Domain.Models;
using SmartMenu.Domain.Models.DTO;

namespace SmartMenu.API.Ultility
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Store, StoreCreateDTO>().ReverseMap();
            CreateMap<Store, StoreUpdateDTO>().ReverseMap();
            CreateMap<Template, TemplateCreateDTO>().ReverseMap();
            CreateMap<Layer, LayerCreateDTO>().ReverseMap();
            CreateMap<Layer, LayerUpdateDTO>().ReverseMap();
            CreateMap<Box, BoxCreateDTO>().ReverseMap();
            CreateMap<Box, BoxUpdateDTO>().ReverseMap();
        }
    }
}
