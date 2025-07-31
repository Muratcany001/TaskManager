using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Abstract
{
    public interface ITaskVersionRepository
    {
        Task<TaskVersion> GetNewVersionByTaskId(int taskId, int lastUpdaterId, string status);
        Task<int> GetLatestVersionByTaskId(int id);
        Task<TaskVersion> DeleteLatestVersionByTaskId(int id);
        Task<TaskVersion> DeleteVersionByTaskId(int id);
        Task<int?> GetVersionByTaskId(int id);
        Task<TaskVersion> ChangeVersionStatusById(int id, string status);
        Task<TaskVersion> GetBackVersionByVersionNumber(int taskId, int versionId, int lastUpdaterId);
        Task<List<TaskVersion>> GetAllVersionsByTaskId(int id);
        Task<List<TaskVersion>> GetDocumentByTaskId(int taskId);
        Task<List<TaskVersion>> GetAllVersionByTaskId(int taskId);
        Task<TaskVersion> GetVersionByVersionId(int versionId);
    }
}
