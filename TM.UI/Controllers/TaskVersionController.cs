using Microsoft.AspNetCore.Mvc;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.UI.Controllers
{
    [ApiController]
    public class TaskVersionController : ControllerBase
    {
        private readonly ITaskVersionRepository _taskVersionRepository;
        public TaskVersionController(ITaskVersionRepository taskVersionRepository)
        {
            _taskVersionRepository = taskVersionRepository;
        }
        [HttpPost("version/GetNewVersion")]
        public async Task<IActionResult> GetNewVersionByTaskId(int taskId, [FromQuery] int lastUpdaterId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Geçerli durum girin");
            }
            var existedVersion = await _taskVersionRepository.GetVersionByTaskId(taskId);

            try
            {
                var newVersion = await _taskVersionRepository.GetNewVersionByTaskId(
                    taskId, lastUpdaterId, status);
                return CreatedAtAction(
                nameof(GetNewVersionByTaskId),
                new
                {
                    id = newVersion.Id,
                    versionNumber = newVersion.VersionNumber
                },
                newVersion
                );
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("version/DeleteLatestVersion/{taskId}")]
        public async Task<IActionResult> DeleteLastestVersion(int taskId)
        {
            var lastVersion = await _taskVersionRepository.GetLatestVersionByTaskId(taskId);
            if (lastVersion == null)
                return BadRequest("Geçerli versiyon bulunamadı");

            var deleteVersion = await _taskVersionRepository.DeleteLatestVersionByTaskId(taskId);
            if (deleteVersion == null)
                return BadRequest("Daha fazla geri alınamıyor.");
            return Ok(deleteVersion);
        }

        [HttpGet("version/GetLatestVersion/{taskId}")]
        public async Task<IActionResult> GetLatestVersionByTaskId(int taskId)
        {
            var latestVersion = await _taskVersionRepository.GetLatestVersionByTaskId(taskId);
            if (latestVersion == null)
                return BadRequest("Versiyon veya Task bulunamadı");
            return Ok(latestVersion);
        }
        [HttpPost("version/ChangeVersionStatus/{taskId}/{status}")]
        public async Task<IActionResult> ChangeVersionStatusById(int taskId, string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Geçersiz status değeri");

            var existedVersion = await _taskVersionRepository.GetVersionByTaskId(taskId);
            if (existedVersion == null)
                return BadRequest("Geçerli task veya versiyon bulunamadı");

            await _taskVersionRepository.ChangeVersionStatusById(taskId, status);
            var updatedStatus = _taskVersionRepository.GetVersionByTaskId(taskId);
            return Ok(updatedStatus.Status);
        }

        [HttpGet("version/GetVersion/{taskId}")]
        public async Task<ActionResult<int>> GetVersionByTaskId(int taskId)
        {
            var existedVersion = await _taskVersionRepository.GetVersionByTaskId(taskId);
            if (existedVersion == null)
                return NotFound("Geçerli versiyon bulunamadı");
            return Ok(existedVersion);
        }

        [HttpGet("version/GetAllVersionsByTaskId/{taskId}")]
        public async Task<IActionResult> GetAllVersions(int taskId)
        {
            var existedTask = await _taskVersionRepository.GetVersionByTaskId(taskId);
            if (existedTask == null)
                return NotFound("Task bulunamadı");

            var versions = await _taskVersionRepository.GetAllVersionsByTaskId(taskId);
            return Ok(versions);

        }

        [HttpPatch("version/GetBackVersion/{taskId}/{versionId}/{lastUpdaterId}")]
        public async Task<ActionResult> GetBackVersionByVersionNumber(int taskId, int versionId, int lastUpdaterId)
        {
            if (taskId == null)
                return BadRequest("Geçerli taskID bulunamadı");
            if (versionId == null)
                return BadRequest("Geçerli versionId bulunamadı");
            if (lastUpdaterId == null)
                return BadRequest("Geçerli kullanıcı ID'si bulunamadı");

            await _taskVersionRepository.GetBackVersionByVersionNumber(taskId, versionId, lastUpdaterId);

            var existedVersion = await _taskVersionRepository.GetVersionByTaskId(taskId);
            if (existedVersion == null)
                return NotFound("Geçerli task bulunamadı");

            return Ok(existedVersion);
        }
        [HttpGet("version/GetDocumentsByTaskId/{taskId}")]
        public async Task<ActionResult<List<TaskVersion>>> GetDocumentByTaskId(int taskId)
        {
            var versionsWithDocuments = await _taskVersionRepository.GetDocumentByTaskId(taskId);

            if (versionsWithDocuments == null || !versionsWithDocuments.Any())
                return NotFound("Belge içeren versiyon bulunamadı.");

            return Ok(versionsWithDocuments);
        }
        [HttpGet("version/GetAllVerisonByTaskId/{taskId}")]
        public async Task<ActionResult<List<TaskVersion>>> GetAllVersionByTaskId(int taskId)
        {
            var existedVersions = await _taskVersionRepository.GetAllVersionByTaskId(taskId);
            if (!existedVersions.Any())
                return NotFound("Geçerli taska ait versiyon bulunamadı");

            return existedVersions;
        }
        [HttpGet("version/GetVersionByVersionId/{versionId}")]
        public async Task<ActionResult> GetVersionByVersionId(int versionId)
        {
            var existedVersion = await _taskVersionRepository.GetVersionByVersionId(versionId);
            if (existedVersion == null)
                return NotFound("Geçerli versiyon bulunamadı");

            return Ok(existedVersion);
        }        

            }
 }

