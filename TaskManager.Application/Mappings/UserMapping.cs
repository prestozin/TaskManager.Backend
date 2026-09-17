using Mapster;
using TaskManager.Application.DTOs.User;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Mappings;

public class UserMapping
{
    public void RegisterMapping(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserResponseDto>();
    }
}
