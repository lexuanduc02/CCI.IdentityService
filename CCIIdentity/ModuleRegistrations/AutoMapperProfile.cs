using AutoMapper;
using CCI.Domain.Entities;
using CCI.Model.OAuthModels;
using CCI.Model.Users;

namespace CCIIdentity;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<UpdateUserRequest, User>().ReverseMap();
        CreateMap<RegisterModel, User>().ReverseMap();
        CreateMap<User, UserViewModel>().ReverseMap();
    }
}
