using Dtos.VersionDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.BLL.ViewModels;
using TM.DAL.Concrete;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Services.VersionService
{
    public interface IVersionService 
    {
        
        
        Task<ResultViewModel<TaskVersion>> DeleteVersionByTaskId(int id);
        Task<ResultViewModel<List<TaskVersion>>> GetAllVersionByTaskId(int taskId);

        Task<ResultViewModel<string>> ChangeVersionStatusById(int versionId, string status);
        Task<ResultViewModel<TaskVersion>> DeleteLatestVersionByTaskId(int id);

        Task<ResultViewModel<int>> GetLatestVersionByTaskId(int id);
        Task<ResultViewModel<int>> GetVersionByTaskId(int id);
        Task<ResultViewModel<TaskVersion>> GetNewVersionByTaskId(CreateVersionDto createVersionDto);
        Task<ResultViewModel<List<TaskVersion>>> GetAllVersionsByTaskId(int id);
        Task<ResultViewModel<TaskVersion>> GetBackVersionByVersionNumber(GetBackVersionDto getBackVersionDto);

        Task<ResultViewModel<List<TaskVersion>>> GetDocumentByTaskId(int taskId);
        Task<ResultViewModel<TaskVersion>> GetVersionByVersionId(int versionId);
    }
}
