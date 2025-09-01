using AutoMapper;
using Dtos.VersionDtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.ViewModels;
using TM.DAL;
using TM.DAL.Abstract;
using TM.DAL.Concrete;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.VersionService
{
    public class VersionService : IVersionService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<TaskVersion> _baseRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskVersionRepository _taskVersionRepository;
        private readonly IMapper _mapper;
        private readonly Context _context;
        
        public VersionService(IBaseRepository<TaskVersion> baseRepository, IMapper mapper, Context context, ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
            _baseRepository = baseRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ResultViewModel<string>> ChangeVersionStatusById(int versionId, string status)
        {
            var version = await _taskVersionRepository.ChangeVersionStatusById(versionId, status);
            return ResultViewModel<string>.Success(status, "Status changed succesfully", 200);
        }

        public async Task<ResultViewModel<TaskVersion>> DeleteLatestVersionByTaskId(int taskId)
        {
            
            var currentTaskVersion = await _baseRepository.GetAsync(
                predicate: x => x.TaskId == taskId,
                includeFunc: query => query
                    .Include(x => x.Task) 
                        .ThenInclude(t => t.Versions) 
                            .ThenInclude(v => v.Documents)
                    .Include(x => x.Documents)
            );

            if (currentTaskVersion?.Task == null)
                return ResultViewModel<TaskVersion>.Failure("Task not found.");

            var task = currentTaskVersion.Task;

            var latestVersion = task.Versions
                .OrderByDescending(x => x.VersionNumber)
                .FirstOrDefault();

            if (latestVersion == null)
                return ResultViewModel<TaskVersion>.Failure("No version found to delete.");

            var backVersion = task.Versions
                .Where(x => x.Id != latestVersion.Id)
                .OrderByDescending(x => x.VersionNumber)
                .FirstOrDefault();

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Versions.Remove(latestVersion);

                if (backVersion != null)
                {
                    task.CurrentVersionId = backVersion.Id;
                    _context.Entry(task).Property(x => x.CurrentVersionId).IsModified = true;
                }
                else
                {
                    task.CurrentVersionId = null;
                    _context.Entry(task).Property(x => x.CurrentVersionId).IsModified = true;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ResultViewModel<TaskVersion>.Success(latestVersion, "Latest version deleted successfully.", 200);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ResultViewModel<TaskVersion>.Failure($"An error occurred while deleting the latest version: {ex.Message}");
            }
        }

        public async Task<ResultViewModel<TaskVersion>> DeleteVersionByTaskId(int id)
        {
            var deletedVersion = await _baseRepository.GetByIdAsync(id);
            await _baseRepository.DeleteAsync(deletedVersion);
            return ResultViewModel<TaskVersion>.Success(deletedVersion , "Version deleted succesfully", 200);
        }

        public async Task<ResultViewModel<List<TaskVersion>>> GetAllVersionByTaskId(int taskId)
        {
            var versions = await _baseRepository.GetListAsync(
                predicate: tv => tv.TaskId == taskId,
                asNoTracking: true
             );

            return ResultViewModel<List<TaskVersion>>.Success(versions, "List created succesfully", 200);
        }

        public async Task<ResultViewModel<List<TaskVersion>>> GetAllVersionsByTaskId(int id)
        {
            var versions = await _baseRepository.GetListAsync(tv => tv.TaskId == id);
            return ResultViewModel<List<TaskVersion>>.Success(versions," List created succesfully", 200);
        }

        public Task<ResultViewModel<TaskVersion>> GetBackVersionByVersionNumber(GetBackVersionDto getBackVersionDto)
        {
            var backVersion = _taskVersionRepository.GetBackVersionByVersionNumber(getBackVersionDto);
            return backVersion;
        }

        public Task<ResultViewModel<List<TaskVersion>>> GetDocumentByTaskId(int taskId)
        {
            var document = _taskVersionRepository.GetDocumentByTaskId(taskId);
            return document;
        }

        public Task<ResultViewModel<int>> GetLatestVersionByTaskId(int id)
        {
            var version = _taskVersionRepository.GetLatestVersionByTaskId(id);
            return version;
        }

        public Task<ResultViewModel<TaskVersion>> GetNewVersionByTaskId(CreateVersionDto createVersionDto)
        {
            throw new NotImplementedException();
        }

        public Task<ResultViewModel<int>> GetVersionByTaskId(int id)
        {
            var version = _taskVersionRepository.GetLatestVersionByTaskId(id);
            return version;
        }

        public Task<ResultViewModel<TaskVersion>> GetVersionByVersionId(int versionId)
        {
            var version = _taskVersionRepository.GetVersionByVersionId(versionId);
            return version;
        }
    }
}
