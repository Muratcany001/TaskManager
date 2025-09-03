using AutoMapper;
using Dtos.DocumentDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TM.BLL.Services.GoogleDriveService;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Concrete;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.DocumentService
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentService _documentService;
        private readonly IBaseRepository<Document> _baseRepository;
        private readonly IMapper _mapper;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly Context _context;

        public async Task<Document> CreateDocument(CreateDocumentDto createDocumentDto)
        {
            var existedVersion = await _context.Tasks
                                   .Include(x => x.CurrentVersion)
                                   .FirstOrDefaultAsync(x => x.Id == createDocumentDto.taskId);

            if (existedVersion == null)
                return null;

            var document = new Document
            {
                TaskId = createDocumentDto.taskId,
                Title = createDocumentDto.title

            };

            if (createDocumentDto.file != null)
            {
                var uploadedFilePath = await _googleDriveService.UploadFileAsync(createDocumentDto.file);
                document.FilePath = uploadedFilePath;
            }

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }
        public async Task<Document> UpdateDocumentFilePathById(UpdateDocumentDto updateDocumentDto)
        {
            var existedItem = await _context.Documents.FindAsync(updateDocumentDto.id);
            if (existedItem == null) return null;

            if (updateDocumentDto.file != null)
            {
                var match = Regex.Match(existedItem.FilePath, @"/d/([^/]+)");
                if (!match.Success)
                {
                    throw new InvalidOperationException("Google Drive dosya linki geçersiz.");
                }
                var fileId = match.Groups[1].Value;

                var uploadedFilePath = await _googleDriveService.UpdateFileAsync(fileId, updateDocumentDto.file);
                existedItem.FilePath = uploadedFilePath;
            }

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
        public async Task<Document?> GetDocumentById(int id)
        {
            return await _context.Documents
                .Include(d => d.Task)
                .Include(d => d.TaskVersionId)
                .FirstOrDefaultAsync(d => d.Id == id);
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

