using Microsoft.AspNetCore.Mvc;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;
using TM.DAL;
using Microsoft.EntityFrameworkCore;
using TM.BLL.Services.TaskService;
using Dtos;
namespace TM.UI.Controllers
{
        [ApiController]
        public class UserTaskController : ControllerBase
        {
            private readonly ITaskService _taskService;
            public UserTaskController(ITaskService taskService)
            {
                _taskService = taskService;

        }
            [HttpPost("task/CreateTask")]
            public async Task<ActionResult<UserTask>> CreateTask(CreateTaskDto createTaskDto, [FromQuery] int createdByUserId)
            {
                if (createTaskDto == null)
                    return BadRequest("Geçersiz girdi");
                
                try
                {
                    var savedTask = await _taskService.CreateTask(createTaskDto);
                    return Ok(savedTask);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            [HttpPatch("task/UpdateTaskById/{id}")]
            public async Task<ActionResult> UpdateTask(int id, UpdateTaskDto updateTaskDto)
            {
                if (updateTaskDto == null)
                    return BadRequest("Geçersiz girdi");
                var existedTask = await _taskService.UpdateTaskById(id, updateTaskDto);
                return Ok("Güncelleme başarılı");
            }
            [HttpDelete("task/DeleteTaskById/{id}")]
            public async Task<ActionResult> DeleteTask(int id)
            {
                await _taskService.DeleteTaskById(id);
                return Ok("Silme başarılı");
            }
            [HttpGet("task/GetAllTask")]
            public async Task<IActionResult> GetAllTask()
            {
                var list = await _taskService.GetAllTasks();
                return Ok(list);
            }
            [HttpGet("task/GetTaskById/{id}")]
            public async Task<IActionResult> GetTaskById(int id)
            {
                return await _taskService.GetTaskById(id) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }

            [HttpGet("task/GetTaskByVersionId/{versionId}")]
            public async Task<ActionResult> GetTaskByVersionId(int versionId)
            {
                return await _taskService.GetTaskByVersionId(versionId) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }

            [HttpGet("task/GetTaskByVersion/{version}")]
            public async Task<ActionResult> GetTaskByVersion(int version)
            {
                return await _taskService.GetTaskByVersion(version) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }

            [HttpGet("task/GetTaskByTitle/{title}")]
            public async Task<ActionResult> GetTaskByTitle(string title)
            {
                return await _taskService.GetTaskByTitle(title) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }

            [HttpGet("task/GetTaskByDate/{date}")]
            public async Task<ActionResult> GetTaskByDate(string date)
            {
                return await _taskService.GetTaskByDate(date) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }

            [HttpGet("task/GetLastUpdaterNameById/{taskId}")]
            public async Task<ActionResult> GetLastUpdater(int taskId)
            {
                return await _taskService.GetLastUpdaterNameById(taskId) is { } result ? Ok(result) : NotFound("Son kullanıcı bulunamadı");
            }

            [HttpGet("task/GetFirstUpdaterNameById/{taskId}")]
            public async Task<ActionResult> GetFirstUpdater(int taskId)
            {
                return await _taskService.GetFirstUpdaterNameById(taskId) is { } result ? Ok(result) : NotFound("Son kullanıcı bulunamadı");
            }

    }
}

