using Dtos;
using TM.BLL.ViewModels;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.TaskService;

public interface ITaskService
{
    Task<ResultViewModel<UserTask>> CreateTask(CreateTaskDto taskDto);
    Task<ResultViewModel<UserTask>> UpdateTaskById(int id, UpdateTaskDto updateTaskDto);
    Task<ResultViewModel<UserTask>> DeleteTaskById(int id);
    Task<ResultViewModel<List<ResultViewModel<UserTask>>>> GetAllTasks();
    Task<ResultViewModel<string>> GetLastUpdaterNameById(int id);
    Task<ResultViewModel<string>> GetFirstUpdaterNameById(int id);
}