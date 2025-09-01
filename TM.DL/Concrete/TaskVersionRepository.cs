using Dtos.VersionDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.ViewModels;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Concrete
{
    public class TaskVersionRepository : ITaskVersionRepository
    {
        private readonly Context _context;
        public TaskVersionRepository(Context context)
        {
            _context = context;
        }

        public async Task<ResultViewModel<TaskVersion>> GetNewVersionByTaskId(int taskId, int LastUpdaterId, string status)
        {
            var userTask = _context.Tasks
                                        .Include(x => x.CurrentVersion)
                                            .ThenInclude(x=> x.Documents)
                                        .Include(x => x.Versions)
                                        .FirstOrDefault(x => x.Id == taskId);

            if (userTask == null)
                throw new ArgumentException("Task bulunamadı");
            int nextVersionNumber = 1;
            if (userTask.Versions != null && userTask.Versions.Any())
                nextVersionNumber = userTask.Versions.Max(x => x.VersionNumber) + 1;
            var currentDocs = userTask.CurrentVersion.Documents?.ToList() ?? new List<Document>();


            var copiedDocuments = currentDocs
                .Select(doc => new Document
                {
                    Title = doc.Title,
                    FilePath = doc.FilePath
                }).ToList();

            var newVersion = new TaskVersion
            {
                TaskId = taskId,
                VersionNumber = nextVersionNumber,
                Time = DateTime.UtcNow,
                Status = status,
                CreatedByUserId = LastUpdaterId,
                Documents = copiedDocuments
            };

            await _context.Versions.AddAsync(newVersion);
            await _context.SaveChangesAsync();

            userTask.CurrentVersion = newVersion;
            userTask.CurrentVersionId = newVersion.Id;
            userTask.LastUpdater = LastUpdaterId;

            _context.Tasks.Update(userTask);
            await _context.SaveChangesAsync();
            return ResultViewModel<TaskVersion>.Success(newVersion, "New version created successfully.", 200);
        }

        public async Task<ResultViewModel<List<TaskVersion>>> GetAllVersionsByTaskId(int taskId)
        {
            var userTask = await _context.Versions
                                            .Include(x => x.Task)
                                            .Where(x => x.TaskId == taskId)
                                            .ToListAsync();
            return ResultViewModel<List<TaskVersion>>.Success(userTask, "Versions retrieved successfully.", 200);
        }

        public async Task<ResultViewModel<TaskVersion>> DeleteLatestVersionByTaskId(int taskId)
        {
            var existedTask = await _context.Tasks
                .Include(x => x.CurrentVersion)
                .Include(x => x.Versions)
                    .ThenInclude(x=> x.Documents)
                .FirstOrDefaultAsync(x => x.Id == taskId);

            if (existedTask == null)
                return null;

            var latestVersion = existedTask.CurrentVersion;

            var backVersion = existedTask.Versions
                .Where(x => x.Id != latestVersion.Id)
                .OrderByDescending(x => x.VersionNumber).FirstOrDefault();

            if (latestVersion == null)
                return null;

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Versions.Remove(latestVersion);

                if (backVersion != null)
                {
                    existedTask.CurrentVersionId = backVersion.Id;
                    existedTask.CurrentVersion = backVersion;

                    var restoredDocuments = backVersion.Documents.ToList();
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return ResultViewModel<TaskVersion>.Success(latestVersion, "Latest version deleted successfully.", 200);

            }
            catch
            {
                await transaction.RollbackAsync();
                
                return ResultViewModel<TaskVersion>.Failure("An error occurred while deleting the latest version.");
            }
        }

        public async Task<ResultViewModel<int>> GetLatestVersionByTaskId(int id)
        {

            var latestVersion = await _context.Tasks.Where(x => x.Id == id).Select(x => x.CurrentVersion.VersionNumber).FirstOrDefaultAsync();
            return ResultViewModel<int>.Success(latestVersion, "Latest version number retrieved successfully.", 200);
        }


        public async Task<ResultViewModel<TaskVersion?>> GetVersionByVersionId(int versionId)
        {
            var version = await _context.Versions
                                        .Include(x => x.Documents)
                                        .FirstOrDefaultAsync(x => x.Id == versionId);

            return ResultViewModel<TaskVersion?>.Success(version, "Version retrieved successfully.", 200);
        }


        public async Task<ResultViewModel<TaskVersion>> GetBackVersionByVersionNumber(GetBackVersionDto getBackVersionDto)
        {
            var versionId = getBackVersionDto.taskId;
            var existedTask = await _context.Tasks.
                Include(x => x.Versions)
                .Include(x => x.CurrentVersion)
                .FirstOrDefaultAsync(x => x.Id == versionId);
            if (existedTask == null)
            {
                throw new ArgumentException("Geçerli ID ile görev bulunamadı");
            }

            var existedVersion = await _context.Versions.FindAsync(versionId);

            _context.Tasks.Update(existedTask);
            await _context.SaveChangesAsync();
            return ResultViewModel<TaskVersion?>.Success(existedVersion, "Version reverted succesfully",200);
        }

        public async Task<ResultViewModel<string>> ChangeVersionStatusById(int versionId, string status)
        {
            var existedVersion = await _context.Versions.FindAsync(versionId);
            if (existedVersion == null)
                return null;

            existedVersion.Status = status;
            _context.Update(existedVersion);
            await _context.SaveChangesAsync();
            return ResultViewModel<string>.Success(existedVersion.Status, "Status changed succesfully", 200); 
        }
        public async Task<ResultViewModel<List<TaskVersion>>> GetDocumentByTaskId(int taskId)
        {
            var result = await _context.Versions
                .Where(tv => tv.TaskId == taskId)
                .Include(tv => tv.Documents)
                .ToListAsync();

            return ResultViewModel<List<TaskVersion>>.Success(result, "Document found succesfully", 200);
        }
    }
}

