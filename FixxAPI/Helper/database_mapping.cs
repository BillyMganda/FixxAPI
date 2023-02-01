using AutoMapper;
using FixxAPI.DTOs;
using FixxAPI.Models;

namespace FixxAPI.Helper
{
    public class database_mapping : Profile
    {
        public database_mapping()
        {
            CreateMap<property_category, property_category_return_dto>();
            CreateMap<property_type, property_type_return_dto>();
        }
    }
}
