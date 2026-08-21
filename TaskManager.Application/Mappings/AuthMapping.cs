using TaskManager.Application.DTOs;
using TaskManager.Core.Entities;
using Mapster;
namespace TaskManager.Application.Mappings;

public class AuthMapping 
{
    public void RegisterMapping(TypeAdapterConfig config)
    {
        config.NewConfig<CreateUserDto, User>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.HashPassword)
            .Ignore(dest => dest.CreatedAt);

        config.NewConfig<User, LoginResponseDto>();
    }
}
