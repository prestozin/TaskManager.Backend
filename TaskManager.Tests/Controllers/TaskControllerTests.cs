using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManager.Api.Controllers;
using TaskManager.Application.DTOs;
using TaskManager.Application.DTOs.Task.Request;
using TaskManager.Application.DTOs.Task.Response;
using TaskManager.Application.Interfaces;
using TaskManager.Core.Shared;

namespace TaskManager.Tests.Controllers;

public class TaskControllerTests
{
    private readonly Mock<ITaskService> _taskService = new Mock<ITaskService>();

    [Fact]
    public async Task ShouldReturnOk_WhenTaskExists()
    {
        Guid taskId = Guid.NewGuid();

        _taskService
            .Setup(service => service.GetTaskByIdAsync(taskId))
            .ReturnsAsync(ResultResponse<TaskResponse>.Success(new TaskResponse { Id = taskId }));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.GetByIdAsync(taskId);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        Guid taskId = Guid.NewGuid();

        _taskService
            .Setup(service => service.GetTaskByIdAsync(taskId))
            .ReturnsAsync(ResultResponse<TaskResponse>.Failure("Not found"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.GetByIdAsync(taskId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenPagedRequestIsProcessed()
    {
        TaskPagedParams request = new TaskPagedParams();

        _taskService
            .Setup(service => service.GetPagedAsync(request))
            .ReturnsAsync(ResultResponse<PagedResultDto<TaskResponse>>.Success(
                new PagedResultDto<TaskResponse>([], 1, 10, 0)
            ));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.GetPagedAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenCreateTaskSucceeds()
    {
        CreateTaskRequest request = new CreateTaskRequest();

        _taskService
            .Setup(service => service.CreateTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Created"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.CreateTaskAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenCreateTaskFails()
    {
        CreateTaskRequest request = new CreateTaskRequest();

        _taskService
            .Setup(service => service.CreateTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Failed"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.CreateTaskAsync(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenEditTaskSucceeds()
    {
        EditTaskRequest request = new EditTaskRequest();

        _taskService
            .Setup(service => service.EditTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Updated"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.EditTaskAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenEditTaskFails()
    {
        EditTaskRequest request = new EditTaskRequest();

        _taskService
            .Setup(service => service.EditTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Not found"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.EditTaskAsync(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenDeleteTaskSucceeds()
    {
        DeleteTaskRequest request = new DeleteTaskRequest { TaskId = [Guid.NewGuid()] };

        _taskService
            .Setup(service => service.DeleteTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Success("Deleted"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.DeleteTaskAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenDeleteTaskFails()
    {
        DeleteTaskRequest request = new DeleteTaskRequest { TaskId = [Guid.NewGuid()] };

        _taskService
            .Setup(service => service.DeleteTaskAsync(request))
            .ReturnsAsync(ResultResponse<string>.Failure("Not found"));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.DeleteTaskAsync(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenSelectablesAreRequested()
    {
        _taskService
            .Setup(service => service.GetSelectablesAsync())
            .ReturnsAsync(ResultResponse<TaskSelectablesResponse>.Success(new TaskSelectablesResponse()));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.GetSelectablesAsync();

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenReportIsRequested()
    {
        ReportPagedParams request = new ReportPagedParams();

        _taskService
            .Setup(service => service.GetReportAsync(request))
            .ReturnsAsync(ResultResponse<TaskReportResponse>.Success(new TaskReportResponse()));

        TaskController controller = new TaskController(_taskService.Object);

        var result = await controller.GetReportAsync(request);

        Assert.IsType<OkObjectResult>(result);
    }
}
