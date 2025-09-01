using Dtos.VersionDtos;
using Microsoft.AspNetCore.Mvc;
using TM.BLL.Services.VersionService;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.UI.Controllers
{
    [ApiController]
    public class TaskVersionController : ControllerBase
    {
        private readonly ITaskVersionRepository _taskVersionRepository;
        private readonly IVersionService _versionService;
        public TaskVersionController(ITaskVersionRepository taskVersionRepository, IVersionService versionService)
        {
            _taskVersionRepository = taskVersionRepository;
            _versionService = versionService;
        }
        [HttpPost("version/GetNewVersion")]
        public async Task<IActionResult> GetNewVersionByTaskId(CreateVersionDto createVersionDto)
        {
            if (string.IsNullOrEmpty(createVersionDto.Status))
            {
                return BadRequest("Geçerli durum girin");
            }

            try
            {
                var newVersion = await _versionService.GetNewVersionByTaskId(createVersionDto);

                if (newVersion == null || !newVersion.IsSuccess)
                {
                    return BadRequest(newVersion?.Message ?? "Version oluşturulamadı");
                }

                return CreatedAtAction(
                    nameof(GetVersionByVersionId),
                    new { versionId = newVersion.Data.Id },
                    newVersion
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("version/DeleteLatestVersion/{taskId}")]
        public async Task<IActionResult> DeleteLatestVersion(int taskId)
        {
            var lastVersion = await _versionService.GetLatestVersionByTaskId(taskId);
            if (lastVersion == null)
                return BadRequest("Geçerli versiyon bulunamadı");

            var deleteVersion = await _versionService.DeleteLatestVersionByTaskId(taskId);
            if (deleteVersion == null)
                return BadRequest("Daha fazla geri alınamıyor.");

            return Ok(deleteVersion);
        }

        [HttpGet("version/GetLatestVersion/{taskId}")]
        public async Task<IActionResult> GetLatestVersionByTaskId(int taskId)
        {
            var latestVersion = await _versionService.GetLatestVersionByTaskId(taskId);
            if (latestVersion == null)
                return BadRequest("Versiyon veya Task bulunamadı");

            return Ok(latestVersion);
        }

        [HttpPost("version/ChangeVersionStatus/{versionId}/{status}")]
        public async Task<IActionResult> ChangeVersionStatusById(int versionId, string status)
        {
            if (string.IsNullOrEmpty(status))
                return BadRequest("Geçersiz status değeri");

            var existedVersion = await _versionService.GetVersionByVersionId(versionId);
            if (existedVersion == null)
                return BadRequest("Geçerli task veya versiyon bulunamadı");

            var updatedStatus = await _versionService.ChangeVersionStatusById(versionId, status);
            return Ok(updatedStatus);
        }

        [HttpGet("version/GetVersion/{taskId}")]
        public async Task<ActionResult<int>> GetVersionByTaskId(int taskId)
        {
            var existedVersion = await _versionService.GetVersionByTaskId(taskId);
            if (existedVersion == null)
                return NotFound("Geçerli versiyon bulunamadı");

            return Ok(existedVersion);
        }
        [HttpPatch("version/GetBackVersion")]
        public async Task<ActionResult> GetBackVersionByVersionNumber([FromBody] GetBackVersionDto getBackVersionDto)
        {
            if (getBackVersionDto.taskId == null)
                return BadRequest("Geçerli taskID bulunamadı");
            if (getBackVersionDto.versionId == null)
                return BadRequest("Geçerli versionId bulunamadı");
            if (getBackVersionDto.LastUpdaterId == null)
                return BadRequest("Geçerli kullanıcı ID'si bulunamadı");

            await _versionService.GetBackVersionByVersionNumber(getBackVersionDto);

            var existedVersion = await _versionService.GetVersionByTaskId(getBackVersionDto.taskId);
            if (existedVersion == null)
                return NotFound("Geçerli task bulunamadı");

            return Ok(existedVersion);
        }

        [HttpGet("version/GetDocumentsByTaskId/{taskId}")]
        public async Task<ActionResult<List<TaskVersion>>> GetDocumentByTaskId(int taskId)
        {
            var versionsWithDocuments = await _versionService.GetDocumentByTaskId(taskId);

            if (versionsWithDocuments == null)
                return NotFound("Belge içeren versiyon bulunamadı.");

            return Ok(versionsWithDocuments);
        }

        [HttpGet("version/GetAllVersionsByTaskId/{taskId}")]
        public async Task<ActionResult<List<TaskVersion>>> GetAllVersionsByTaskId(int taskId)
        {
            var existedTask = await _versionService.GetVersionByTaskId(taskId);
            if (existedTask == null)
                return NotFound("Task bulunamadı");

            var versions = await _versionService.GetAllVersionsByTaskId(taskId);
            return Ok(versions);
        }

        [HttpGet("version/GetVersionByVersionId/{versionId}")]
        public async Task<ActionResult> GetVersionByVersionId(int versionId)
        {
            var existedVersion = await _versionService.GetVersionByVersionId(versionId);
            if (existedVersion == null)
                return NotFound("Geçerli versiyon bulunamadı");

            return Ok(existedVersion);
        }
    }
}
