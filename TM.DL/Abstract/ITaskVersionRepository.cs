using Dtos.VersionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.ViewModels;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Abstract
{
    public interface ITaskVersionRepository
    {
        
        Task<ResultViewModel<int>> GetLatestVersionByTaskId(int id);
        Task<ResultViewModel<string>> ChangeVersionStatusById(int versionId, string status);
        Task<ResultViewModel<TaskVersion>> GetBackVersionByVersionNumber(GetBackVersionDto getBackVersionDto);
        
        Task<ResultViewModel<List<TaskVersion>>> GetDocumentByTaskId(int taskId);
        Task<ResultViewModel<TaskVersion>> GetVersionByVersionId(int versionId);
    }
}
