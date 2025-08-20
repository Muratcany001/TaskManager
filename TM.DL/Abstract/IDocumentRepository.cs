using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;
namespace TM.DAL.Abstract
{
    public interface IDocumentRepository
    {
        Task<Document> CreateDocument(int taskId, string title, IFormFile file);
        Task<Document> UpdateDocumentFilePathById(int id, IFormFile file);
        Task<Document> DeleteDocumentById(int id);
        Task<Document> GetDocumentById(int id);
        Task<List<Document>> GetAllDocuments();
    }
}
