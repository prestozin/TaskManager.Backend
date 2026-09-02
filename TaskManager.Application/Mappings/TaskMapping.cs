using TaskManager.Application.DTOs;
using TaskManager.Core.Entities;
using Mapster;
using TaskManager.Application.DTOs.Task;
namespace TaskManager.Application.Mappings;

public class TaskMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateTaskDto, TaskEntity>()
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UserId!);


        config.NewConfig<TaskEntity, TaskResponseDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt.ToString("yyyy-MM-dd"))
            .Map(dest => dest.Status, src => src.TaskStatus!.Description)
            .Map(dest => dest.Priority, src => src.TaskPriority!.Description);

        config.NewConfig<EditTaskDto, TaskEntity>()
           .Ignore(dest => dest.CreatedAt)
           .Ignore(dest => dest.Id!);
    }
}
