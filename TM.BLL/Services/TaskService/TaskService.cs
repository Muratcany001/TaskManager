using AutoMapper;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using TM.BLL.ViewModels;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Concrete;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.TaskService;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<UserTask> _baseRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IMapper _mapper;
    private readonly Context _context;
    public TaskService(IBaseRepository<UserTask> baseRepository, IMapper mapper, Context context, ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
        _baseRepository = baseRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ResultViewModel<UserTask>> CreateTask(CreateTaskDto taskDto)
    {
        var entity = _mapper.Map<UserTask>(taskDto);
        await _baseRepository.AddAsync(entity);
        return ResultViewModel<UserTask>.Success(entity, "Task created successfully.", 201);
    }

    public async Task<ResultViewModel<UserTask>> DeleteTaskById(int id)
    {
        var entity = await _baseRepository.GetByIdAsync(id);
        await _baseRepository.DeleteAsync(entity);
        return ResultViewModel<UserTask>.Success(entity, "Task deleted successfully.", 200);
    }

    public async Task<ResultViewModel<List<UserTask>>> GetAllTasks()
    {
        var tasks = await _baseRepository.GetListAsync();
        return ResultViewModel<List<UserTask>>.Success(tasks, "List created successfully", 200);
    }

    public async Task<ResultViewModel<string>> GetFirstUpdaterNameById(int id)
    {
        var existedTask = await _baseRepository.GetAsync(x => x.Id == id, includeFunc: query => query.Include(t => t.FirstUpdater));
        var user = await _userRepository.GetByIdAsync(existedTask.FirstUpdater);
        return ResultViewModel<string>.Success(user.Email, "First updater name retrieved successfully.", 200);
    }

    public async Task<ResultViewModel<string>> GetLastUpdaterNameById(int id)
    {
        var existedTask = await _baseRepository.GetAsync(x => x.Id == id, includeFunc: query => query.Include(t => t.LastUpdater));
        var user = await _userRepository.GetByIdAsync(existedTask.LastUpdater);
        return ResultViewModel<string>.Success(user?.Email, "Last updater name retrieved successfully.", 200);
    }

    public Task<ResultViewModel<UserTask>> GetTaskByDate(string date)
    {
        var data = _taskRepository.GetTaskByDate(date);
        return data;
    }

    public Task<ResultViewModel<UserTask>> GetTaskById(int id)
    {
        var data = _taskRepository.GetTaskById(id);
        return data;
    }

    public Task<ResultViewModel<UserTask>> GetTaskByTitle(string title)
    {
        var data = _taskRepository.GetTaskByTitle(title);
        return data;
    }

    public Task<ResultViewModel<UserTask>> GetTaskByVersion(int version)
    {
        var data = _taskRepository.GetTaskByVersion(version);
        return data;
    }

    public Task<ResultViewModel<UserTask>> GetTaskByVersionId(int versionId)
    {
        var data = _taskRepository.GetTaskByVersionId(versionId);
        return data;
    }

    public async Task<ResultViewModel<UserTask>> UpdateTaskById(int id, UpdateTaskDto updateTaskDto)
    {

        var exsitedTask = await _baseRepository.GetByIdAsync(id);
        _mapper.Map(updateTaskDto, exsitedTask);
        await _baseRepository.UpdateAsync(exsitedTask);
        return ResultViewModel<UserTask>.Success(exsitedTask, "Task updated successfully.", 200);

    }
}