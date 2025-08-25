using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dtos;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;
using TM.BLL.ViewModels;

namespace TM.DAL.Concrete
{
    public class UserTaskRepository : BaseRepository<UserTask>,  ITaskRepository
    {
        private readonly Context _context;

        public UserTaskRepository(Context context) : base(context)
        {
            _context = context;
        }

        public async Task<ResultViewModel<UserTask>> GetTaskById(int id)
        {
            try
            {
                var task = await GetAsync(
                    predicate: t => t.Id == id,
                    includeFunc: query => query

                        .Include(t => t.CurrentVersion)
                        .Include(t => t.Versions)
                        .Include(t => t.Documents)
                        );

                if (task == null)
                {
                    return new ResultViewModel<UserTask>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Task not found"
                    };
                }

                return new ResultViewModel<UserTask>
                {
                    Data = task,
                    IsSuccess = true,
                    Message = "Task retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<UserTask>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = $"Failed to get task: {ex.Message}"
                };
            }
        }

        public async Task<ResultViewModel<UserTask>> GetTaskByVersionId(int versionId)
        {
            try
            {
                var task = await GetAsync(
                predicate: t => t.CurrentVersionId == versionId,
                includeFunc: q => q
                    .Include(t => t.CurrentVersion)
                    .Include(t => t.Versions)
                    .Include(t => t.Documents)
            );

                if (task == null)
                {
                    return new ResultViewModel<UserTask>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Task not found for the given version ID"
                    };
                }

                return new ResultViewModel<UserTask>
                {
                    Data = task,
                    IsSuccess = true,
                    Message = "Task retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<UserTask>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = $"Failed to get task: {ex.Message}"
                };
            }
        }

        public async Task<ResultViewModel<UserTask>> GetTaskByVersion(int version)
        {
            try
            {
                var task = await GetAsync(
                    predicate: t => t.CurrentVersion.VersionNumber == version,
                    includeFunc: q => q
                    .Include(t => t.CurrentVersion)
                    .Include(t => t.Versions)
                    .Include(t => t.Documents)
                    );

                if (task == null)
                {
                    return new ResultViewModel<UserTask>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Task not found for the given version"
                    };
                }

                return new ResultViewModel<UserTask>
                {
                    Data = task,
                    IsSuccess = true,
                    Message = "Task retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<UserTask>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = $"Failed to get task: {ex.Message}"
                };
            }
        }

        public async Task<ResultViewModel<UserTask>> GetTaskByTitle(string title)
        {
            try
            {
                var task = await GetAsync(
                    predicate: t => t.Title.Equals(title),
                    includeFunc: q => q
                    .Include(t => t.CurrentVersion)
                    .Include(t => t.Versions)
                    .Include(t => t.Documents)
                    );

                if (task == null)
                {
                    return new ResultViewModel<UserTask>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Task not found with the given title"
                    };
                }

                return new ResultViewModel<UserTask>
                {
                    Data = task,
                    IsSuccess = true,
                    Message = "Task retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<UserTask>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = $"Failed to get task: {ex.Message}"
                };
            }
        }

        public async Task<ResultViewModel<UserTask>> GetTaskByDate(string date)
        {
            try
            {
                var task = await GetAsync(
                    predicate: t => t.CreateDate.Equals(date),
                    includeFunc: q => q
                    .Include(t => t.CurrentVersion)
                    .Include(t => t.Versions)
                    .Include(t => t.Documents)
                    );

                if (task == null)
                {
                    return new ResultViewModel<UserTask>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = "Task not found for the given date"
                    };
                }

                return new ResultViewModel<UserTask>
                {
                    Data = task,
                    IsSuccess = true,
                    Message = "Task retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResultViewModel<UserTask>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = $"Failed to get task: {ex.Message}"
                };
            }
        }
    }
}