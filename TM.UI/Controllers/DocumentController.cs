using Microsoft.AspNetCore.Mvc;
using TM.DAL.Entities.AppEntities;
using TM.DAL.Entities;
using TM.DAL.Abstract;
namespace TM.UI.Controllers
{
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [HttpPost("Document/CreateDocument")]
        public async Task<ActionResult> CreateTaskItem(int taskId, Document taskItem)
        {
            try
            {
                if (taskItem == null)
                {
                    return BadRequest("Document cannot be null");
                }

                var createdDocument = await _documentRepository.CreateDocument(taskId, taskItem);
                return Ok(createdDocument);
            } 
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("Document/DeleteCoument/{documentId}")]
        public async Task<ActionResult> DeleteDocumentById(int documentId)
        {

            var existedDocument = await _documentRepository.GetDocumentById(documentId);
            if (existedDocument == null)
                return BadRequest("Geçerli belge bulunamadı");

            await _documentRepository.DeleteDocumentById(existedDocument.Id);
            return Ok("Belge silindi");
        }

        [HttpGet("Document/GetDocumentById/{documentId}")]
        public async Task<ActionResult> GetTaskItemById(int documentId)
        {
            var existedPart = await _documentRepository.GetDocumentById(documentId);
            if (existedPart == null)
            {
                return BadRequest("İstenilen dosya bulunamadı");
            }
            return Ok(existedPart);
        }
        [HttpGet("Document/GetAllDocuments")]
        public async Task<ActionResult<List<ActionResult>>> GetAllTaskItems()
        {
            var documents = await _documentRepository.GetAllDocuments();
            if (documents == null)
                return BadRequest("Liste boş");

            return Ok(documents);
        }
        [HttpPatch("Document/UpdateDocumentById/{documentId}")]
        public async Task<ActionResult> UpdateDocumentFilePathById(int documentId, string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return BadRequest("Güncelleme verisi gönderilmedi");

            var updatedItem = await _documentRepository.UpdateDocumentFilePathById(documentId, filePath);

            if (updatedItem == null)
                return NotFound("Dosya bulunamadı");

            return Ok(updatedItem);
        }
        [HttpGet("Document/GetDocumentByTaskId/{taskId}")]
        public async Task<ActionResult<List<Document>>> GetDocumentyByTaskId(int taskId)
        {
            var existedDocument = await _documentRepository.GetDocumentById(taskId);
            if (existedDocument == null)
                return BadRequest("Belge bulunamadı");
            return Ok(existedDocument);
        }
    }
}
