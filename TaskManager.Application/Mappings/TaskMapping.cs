using Mapster;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Report;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Core.Entities;
using TaskManager.Core.Shared;
namespace TaskManager.Application.Mappings;

public class TaskMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CreateTaskRequest, TaskEntity>()
            .Map(dest => dest.Id, src => Guid.NewGuid())
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UserId!);


        config.NewConfig<TaskEntity, TaskResponse>()
            .Map(dest => dest.Status, src => src.TaskStatus!.Name)
            .Map(dest => dest.Priority, src => src.TaskPriority!.Name);

        config.NewConfig<EditTaskRequest, TaskEntity>()
           .Ignore(dest => dest.CreatedAt)
           .Ignore(dest => dest.Id!);

        config.NewConfig<ReportPagedParams, TaskPagedParams>();
    }
}
