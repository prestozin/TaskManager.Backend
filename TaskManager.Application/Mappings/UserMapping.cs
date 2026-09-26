using Mapster;
using TaskManager.Application.DTOs.User.Request;
using TaskManager.Application.DTOs.User.Response;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Mappings;

public class UserMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserResponse>();

        config.NewConfig<EditUserRequest, User>();
    }
}
