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
        public async Task<ActionResult> CreateTaskItem(Document taskItem)
        {
            if (taskItem == null)
            {
                return BadRequest("Dosya kaydı başarısız oldu");
            }

            var document = new Document
            {
                Title = taskItem.Title,
                FilePath = taskItem.FilePath,
                TaskId = taskItem.TaskId,
            };

            await _documentRepository.CreateDocument(document);
            return Ok("Dosya başarıyla eklendi");
        }
        [HttpDelete("Document/DeleteCoument/{documentId}")]
        public async Task<ActionResult> DeleteDocumentById(int id)
        {

            var existedDocument = await _documentRepository.GetDocumentById(id);
            if (existedDocument == null)
                return BadRequest("Geçerli belge bulunamadı");

            await _documentRepository.DeleteDocumentById(existedDocument.Id);
            return Ok("Belge silindi");
        }

        [HttpGet("Document/GetDocumentById/{documentId}")]
        public async Task<ActionResult> GetTaskItemById(int id)
        {
            var existedPart = await _documentRepository.GetDocumentById(id);
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
        public async Task<ActionResult> UpdateTaskItemById(int id, Document updatedData)
        {
            if (updatedData == null)
                return BadRequest("Güncelleme verisi görülmedi");
            var existedItem = await _documentRepository.UpdateDocumentById(id);
            if (existedItem == null)
                return NotFound("Dosya bulunamadı");

            existedItem.Title = string.IsNullOrEmpty(updatedData.Title)
                ? existedItem.Title
                : updatedData.Title;

            return Ok(existedItem);
        }
    }
}
