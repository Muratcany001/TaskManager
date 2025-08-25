using AutoMapper;
using Dtos;
using Microsoft.EntityFrameworkCore;
using TM.BLL.ViewModels;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Concrete;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.TaskService;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<UserTask> _taskRepository;
    private readonly IMapper _mapper;
    private readonly Context _context;
    public TaskService(IBaseRepository<UserTask> taskRepository, IMapper mapper, Context context)
    {
        _taskRepository = taskRepository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ResultViewModel<UserTask>> CreateTask(CreateTaskDto taskDto)
    {
        var entity = _mapper.Map<UserTask>(taskDto);
        await _taskRepository.AddAsync(entity);
        return ResultViewModel<UserTask>.Success(entity, "Task created successfully.", 201);
    }

    public async Task<ResultViewModel<UserTask>> DeleteTaskById(int id)
    {
        var entity = await _taskRepository.GetByIdAsync(id);
        await _taskRepository.DeleteAsync(entity);
        return ResultViewModel<UserTask>.Success(entity, "Task deleted successfully.", 200);
    }

    public async Task<ResultViewModel<List<UserTask>>> GetAllTasks()
    {
        var tasks = await _taskRepository.GetListAsync();
        return ResultViewModel<List<UserTask>>.Success(tasks, "List created successfully", 200);
    }

    public async Task<ResultViewModel<string>> GetFirstUpdaterNameById(int id)
    {
        var existedTask = await _taskRepository.GetAsync(x => x.Id == id, includeFunc: query => query.Include(t => t.FirstUpdater));
        var user = await _context.Users.FindAsync(existedTask.FirstUpdater);
        return ResultViewModel<string>.Success(user?.Name, "First updater name retrieved successfully.", 200);
    }

    public async Task<ResultViewModel<string>> GetLastUpdaterNameById(int id)
    {
        var existedTask = await _taskRepository.GetAsync(x => x.Id == id, includeFunc: query => query.Include(t => t.LastUpdater));
        var user = await _context.Users.FindAsync(existedTask.LastUpdater);
        return ResultViewModel<string>.Success(user?.Name, "Last updater name retrieved successfully.", 200);
    }

    public async Task<ResultViewModel<UserTask>> UpdateTaskById(int id, UpdateTaskDto updateTaskDto)
    {

        var exsitedTask = await _taskRepository.GetByIdAsync(id);
        _mapper.Map(updateTaskDto, exsitedTask);
        await _taskRepository.UpdateAsync(exsitedTask);
        return ResultViewModel<UserTask>.Success(exsitedTask, "Task updated successfully.", 200);

    }
}