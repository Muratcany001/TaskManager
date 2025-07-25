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
        Task<Document> CreateDocument(int taskId ,Document taskItem);
        Task<Document> UpdateDocumentFilePathById(int id,string filePath);
        Task<Document> DeleteDocumentById(int id);
        Task<Document> GetDocumentById(int id);
        Task<List<Document>> GetAllDocuments();
    }
}
