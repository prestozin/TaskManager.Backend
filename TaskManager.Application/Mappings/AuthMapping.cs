using TaskManager.Core.Entities;
using Mapster;
using TaskManager.Application.DTOs.Auth.Response;
using TaskManager.Application.DTOs.Auth.Request;
namespace TaskManager.Application.Mappings;

public class AuthMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateUserRequest, User>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.HashPassword)
            .Ignore(dest => dest.CreatedAt);

        config.NewConfig<User, LoginResponse>();
    }
}
