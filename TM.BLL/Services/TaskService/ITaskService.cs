using Dtos;
using TM.BLL.ViewModels;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.TaskService;

public interface ITaskService
{
    Task<ResultViewModel<UserTask>> CreateTask(CreateTaskDto taskDto);
    Task<ResultViewModel<UserTask>> UpdateTaskById(int id, UpdateTaskDto updateTaskDto);
    Task<ResultViewModel<UserTask>> DeleteTaskById(int id);
    Task<ResultViewModel<List<UserTask>>> GetAllTasks();
    Task<ResultViewModel<string>> GetLastUpdaterNameById(int id);
    Task<ResultViewModel<string>> GetFirstUpdaterNameById(int id);

    Task<ResultViewModel<UserTask>> GetTaskById(int id);
    Task<ResultViewModel<UserTask>> GetTaskByVersionId(int versionId);
    Task<ResultViewModel<UserTask>> GetTaskByVersion(int version);
    Task<ResultViewModel<UserTask>> GetTaskByTitle(string title);
    Task<ResultViewModel<UserTask>> GetTaskByDate(string date);
}