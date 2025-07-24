using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Concrete
{
    public class DocumentRepository  : IDocumentRepository
    {
        private readonly Context _context;
        public DocumentRepository(Context context)
        {
            _context = context;
        }
        public async Task<Document> CreateDocument(Document taskItem)
        {
            _context.Documents.Add(taskItem);
            await _context.SaveChangesAsync();
            return taskItem;
        }
        public async Task<Document> UpdateDocumentById(int id)
        {
            var existedItem = _context.Documents.FirstOrDefault(x => x.Id == id);
            _context.Documents.Update(existedItem);
            await _context.SaveChangesAsync();
            return existedItem;
        }
        public async Task<List<Document>> GetAllDocuments()
        {
            var documents = await _context.Documents.
                Include(x => x.Task)
                .ToListAsync();

            if (documents.Count == 0)
            {
                return null;
            }
            return documents;
        }
        public async Task<Document> GetDocumentById(int id)
        {
            return await _context.Documents.FindAsync(id);
        }
        public async Task<Document> DeleteDocumentById(int id)
        {
            var existedItem = await _context.Documents.FindAsync(id);
            _context.Documents.Remove(existedItem);
            await _context.SaveChangesAsync();
            return existedItem;
        }
    }
}

