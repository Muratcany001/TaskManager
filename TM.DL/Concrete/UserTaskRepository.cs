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
    public class UserTaskRepository : ITaskRepository
    {
        private readonly Context _context;
        public UserTaskRepository(Context context)
        {
            _context = context;
        }

        public async Task<UserTask> CreateTask(UserTask task)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                task.CreatedDate = DateTime.UtcNow;

                await _context.Tasks.AddAsync(task);
                await _context.SaveChangesAsync();


                var firstVersion = new TaskVersion
                {
                    TaskId = task.Id,
                    VersionNumber = 1,
                    Time = DateTime.UtcNow,
                    Status = "Active",
                    CreatedByUserId = task.FirstUpdater
                };

                await _context.Versions.AddAsync(firstVersion);
                await _context.SaveChangesAsync();

                task.CurrentVersionId = firstVersion.Id;
                _context.Tasks.Update(task);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return await _context.Tasks
                    .Include(t => t.CurrentVersion)
                    .Include(t => t.Versions)
                    .Include(t => t.Documents)
                    .FirstOrDefaultAsync(t => t.Id == task.Id);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<UserTask> UpdateTaskById(int id)
        {
            var existedTask = await _context.Tasks.FindAsync(id);
            _context.Tasks.Update(existedTask);
            await _context.SaveChangesAsync();
            return existedTask;
        }
        public async Task<string?> GetFirstUpdaterNameById(int id)
        {
            var existedUserId = await _context.Tasks.Where(x => x.Id == id).Select(x => x.FirstUpdater).FirstOrDefaultAsync();
            var userName = _context.Users.Where(x => x.Id == existedUserId).Select(x => x.Name).FirstOrDefault();
            return userName;
        }
        public async Task<string?> GetLastUpdaterNameById(int id)
        {
            var existedUserId = await _context.Tasks.Where(x => x.Id == id).Select(x => x.LastUpdater).FirstOrDefaultAsync();
            var userName = _context.Users.Where(x => x.Id == existedUserId).Select(x => x.Name).FirstOrDefault();
            return userName;
        }
        public async Task<UserTask> DeleteTaskById(int taskId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
                return null;

            task.CurrentVersionId = null;
            await _context.SaveChangesAsync();

            var documents = await _context.Documents.Where(d => d.TaskId == taskId).ToListAsync();
            _context.Documents.RemoveRange(documents);

            var versions = await _context.Versions.Where(v => v.TaskId == taskId).ToListAsync();
            _context.Versions.RemoveRange(versions);

            await _context.SaveChangesAsync();

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<List<UserTask>> GetAllTasks()
        {
            return await _context.Tasks
                .Include(x => x.CurrentVersion)
                .Include(x => x.Documents)
                .AsSplitQuery()
                .ToListAsync();
        }
        public async Task<UserTask> GetTaskById(int id)
        {
            return await _context.Tasks.FindAsync(id);
        }
        public async Task<UserTask> GetTaskByVersionId(int VersionId)
        {
            return await _context.Tasks.FirstOrDefaultAsync(x => x.CurrentVersionId == VersionId);
        }
        public async Task<UserTask> GetTaskByVersion(int version)
        {

            return await _context.Tasks.FirstOrDefaultAsync(x => x.CurrentVersion.VersionNumber == version);
        }
        public async Task<UserTask> GetTaskByTitle(string title)
        {
            return await _context.Tasks.FirstOrDefaultAsync(x => x.Title.Equals(title));
        }
        public async Task<UserTask> GetTaskByDate(string date)
        {
            return (await _context.Tasks.FirstOrDefaultAsync(x => x.CreatedDate.Equals(date)));
        }
    }
}
