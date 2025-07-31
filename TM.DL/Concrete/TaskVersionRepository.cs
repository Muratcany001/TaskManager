using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<TaskVersion> GetNewVersionByTaskId(int taskId, int LastUpdaterId, string status)
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
            return newVersion;
        }

        public async Task<List<TaskVersion>> GetAllVersionsByTaskId(int taskId)
        {
            var userTask = await _context.Versions
                                            .Include(x => x.Task)
                                            .Where(x => x.TaskId == taskId)
                                            .ToListAsync();
            return userTask;
        }

        public async Task<TaskVersion> DeleteLatestVersionByTaskId(int taskId)
        {
            var existedTask = await _context.Tasks
                .Include(x => x.CurrentVersion)
                .Include(x => x.Versions)
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
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return latestVersion;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<TaskVersion> DeleteVersionByTaskId(int id)
        {
            var existedVersion = await _context.Versions.FindAsync(id);
            if (existedVersion == null)
                return null;

            _context.Versions.Remove(existedVersion);
            await _context.SaveChangesAsync();
            return existedVersion;
        }

        public async Task<int> GetLatestVersionByTaskId(int id)
        {

            var latestVersion = await _context.Tasks.Where(x => x.Id == id).Select(x => x.CurrentVersion.VersionNumber).FirstOrDefaultAsync();
            return latestVersion;
        }

        public async Task<int?> GetVersionByTaskId(int id)
        {
            var existedTask = await _context.Tasks
                                            .Include(x => x.CurrentVersion)
                                            .FirstOrDefaultAsync(x => x.Id == id);

            if (existedTask == null)
                return null;

            if (existedTask.CurrentVersion == null)
                return null;

            return existedTask.CurrentVersion.VersionNumber;
        }

        public async Task<TaskVersion> UpdateVersionByTaskId(int id, Version version)
        {
            var existedVersion = await _context.Versions.FindAsync(id);
            if (existedVersion == null)
                return null;
            _context.Versions.Update(existedVersion);
            await _context.SaveChangesAsync();
            return existedVersion;
        }
        public async Task<TaskVersion?> GetVersionByVersionId(int versionId)
        {
            var version = await _context.Versions
                                        .Include(x => x.Documents)
                                        .FirstOrDefaultAsync(x => x.Id == versionId);

            return version;
        }


        public async Task<TaskVersion> GetBackVersionByVersionNumber(int taskId, int versionId, int lastUpdaterId)
        {
            var existedTask = await _context.Tasks.
                Include(x => x.Versions)
                .Include(x => x.CurrentVersion)
                .FirstOrDefaultAsync(x => x.Id == taskId);
            if (existedTask == null)
            {
                throw new ArgumentException("Geçerli ID ile görev bulunamadı");
            }

            var existedVersion = await _context.Versions.FindAsync(versionId);

            existedTask.CurrentVersionId = versionId;
            existedTask.LastUpdater = lastUpdaterId;

            _context.Tasks.Update(existedTask);
            await _context.SaveChangesAsync();
            return existedVersion;
        }

        public async Task<TaskVersion> ChangeVersionStatusById(int id, string status)
        {
            var existedVersion = await _context.Versions.FindAsync(id);
            if (existedVersion == null)
                return null;

            existedVersion.Status = status;
            await _context.SaveChangesAsync();
            return existedVersion;
        }
        public async Task<List<TaskVersion>> GetAllVersionByTaskId( int taskId)
        {
            var existedTask = await _context.Tasks.FindAsync(taskId);
            if (existedTask == null)
                return null;

            return await _context.Versions
                                    .Where(x => x.TaskId == taskId)
                                    .Include( x=> x.Task)
                                    .ToListAsync();
        }
        public async Task<List<TaskVersion>> GetDocumentByTaskId(int taskId)
        {
            var result = await _context.Versions
                .Where(tv => tv.TaskId == taskId)
                .Include(tv => tv.Documents)
                .ToListAsync();

            return result;
        }
    }
}

