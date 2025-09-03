using Dtos.DocumentDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.DocumentService
{
    public interface IDocumentService
    {
        Task<Document> CreateDocument(CreateDocumentDto createDocumentDto);
        Task<Document> UpdateDocumentFilePathById(UpdateDocumentDto updateDocumentDto);
        Task<Document> DeleteDocumentById(int id);
        Task<Document> GetDocumentById(int id);
        Task<List<Document>> GetAllDocuments();
    }
}
