using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dtos;
using TM.BLL.ViewModels;
using TM.DAL.Abstract;
using TM.DAL.Entities.AppEntities;

namespace TM.DAL.Abstract
{
    public interface ITaskRepository
    {
        Task<ResultViewModel<UserTask>> GetTaskById(int id);
        Task<ResultViewModel<UserTask>> GetTaskByVersionId(int versionId);
        Task<ResultViewModel<UserTask>> GetTaskByVersion(int version);
        Task<ResultViewModel<UserTask>> GetTaskByTitle(string title);
        Task<ResultViewModel<UserTask>> GetTaskByDate(string date);
    }
}
