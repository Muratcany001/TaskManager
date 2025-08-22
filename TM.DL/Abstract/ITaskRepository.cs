using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dtos;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Abstract
{
    public interface ITaskRepository : IBaseRepository<UserTask>
    {
        Task<UserTask> GetTaskById(int id);
        Task<UserTask> GetTaskByVersionId(int versionId);
        Task<UserTask> GetTaskByVersion(int version);
        Task<UserTask> GetTaskByTitle(string title);
        Task<UserTask> GetTaskByDate(string date);
    }
}
