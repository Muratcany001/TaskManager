using Microsoft.AspNetCore.Mvc;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;
using TM.DAL;
using Microsoft.EntityFrameworkCore;
namespace TM.UI.Controllers
{
        [ApiController]
        public class UserTaskController : ControllerBase
        {
            private readonly ITaskRepository _taskRepository;
            public UserTaskController(ITaskRepository taskRepository)
            {
                _taskRepository = taskRepository;
            }
            [HttpPost("task/CreateTask")]
            public async Task<ActionResult<UserTask>> CreateTask([FromBody] UserTask task, [FromQuery] int createdByUserId)
            {
                if (task == null)
                    return BadRequest("Geçersiz girdi");
                task.CreatedDate = DateTime.Now;
                task.FirstUpdater = createdByUserId;
                task.LastUpdater = createdByUserId;

                try
                {
                    var savedTask = await _taskRepository.CreateTask(task);
                    return Ok(savedTask);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            [HttpPatch("task/UpdateTaskById/{id}")]
            public async Task<ActionResult> UpdateTask(int id, [FromBody] UserTask updateTask)
            {
                if (updateTask == null)
                    return BadRequest("Geçersiz girdi");
                var existedTask = await _taskRepository.UpdateTaskById(id);
                return Ok("Güncelleme başarılı");
            }
            [HttpDelete("task/DeleteTaskById/{id}")]
            public async Task<ActionResult> DeleteTask(int id)
            {
                await _taskRepository.DeleteTaskById(id);
                return Ok("Silme başarılı");
            }
            [HttpGet("task/GetAllTask")]
            public async Task<IActionResult> GetAllTask()
            {
                var list = await _taskRepository.GetAllTasks();
                return Ok(list);
            }
            [HttpGet("task/GetTaskById/{id}")]
            public async Task<IActionResult> GetTaskById(int id)
            {
                return await _taskRepository.GetTaskById(id) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }
            [HttpGet("task/GetTaskByVersionId/{versionId}")]
            public async Task<ActionResult> GetTaskByVersionId(int versionId)
            {
                return await _taskRepository.GetTaskByVersionId(versionId) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }
            [HttpGet("task/GetTaskByVersion/{version}")]
            public async Task<ActionResult> GetTaskByVersion(int version)
            {
                return await _taskRepository.GetTaskByVersion(version) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }
            [HttpGet("task/GetTaskByTitle/{title}")]
            public async Task<ActionResult> GetTaskByTitle(string title)
            {
                return await _taskRepository.GetTaskByTitle(title) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }
            [HttpGet("task/GetTaskByDate/{date}")]
            public async Task<ActionResult> GetTaskByDate(string date)
            {
                return await _taskRepository.GetTaskByDate(date) is { } result ? Ok(result) : NotFound("Task  bulunamadı.");
            }
            [HttpGet("task/GetLastUpdaterNameById/{taskId}")]
            public async Task<ActionResult> GetLastUpdater(int taskId)
            {
                return await _taskRepository.GetLastUpdaterNameById(taskId) is string result ? Ok(result) : NotFound("Son kullanıcı bulunamadı");
            }
            [HttpGet("task/GetFirstUpdaterNameById/{taskId}")]
            public async Task<ActionResult> GetFirstUpdater(int taskId)
            {
                return await _taskRepository.GetFirstUpdaterNameById(taskId) is string result ? Ok(result) : NotFound("Son kullanıcı bulunamadı");
            }

    }
}

