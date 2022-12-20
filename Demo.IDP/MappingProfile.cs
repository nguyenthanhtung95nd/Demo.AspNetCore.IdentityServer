using AutoMapper;
using Demo.IDP.Entities;
using Demo.IDP.Entities.ViewModels;

namespace Demo.IDP
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegistrationModel, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
