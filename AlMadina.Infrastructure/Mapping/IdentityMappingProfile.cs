using AutoMapper;

using AlMadina.Application.DTOs;
using AlMadina.Domain.Entities;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<ApplicationUser, UserDto>();

        CreateMap<RegisterUserDto, ApplicationUser>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email));
    }
}