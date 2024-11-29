using AutoMapper;
using ThamCoCustomerApp.Dtos;
using ThamCoCustomerApp.Models;

namespace ThamCoCustomerApp
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CompanyWithProductDto, ProductViewModel>().ReverseMap();
            CreateMap<OrderDto, OrderViewModel>().ReverseMap();
            CreateMap<CustomerAccountDto, UserProfileViewModel>().ReverseMap();
        }
    }
}
