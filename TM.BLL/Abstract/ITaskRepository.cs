using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TM.DAL.Entities.AppEntities;

namespace TM.BLL.Abstract
{
    public interface ITaskRepository
    {
        Task<UserTask> CreateTask(UserTask userTask);
        Task<UserTask> UpdateTaskById(int id);
        Task<UserTask> DeleteTaskById(int id);
        Task<List<UserTask>> GetAllTasks();
        Task<UserTask> GetTaskById(int id);
        Task<UserTask> GetTaskByVersionId(int versionId);
        Task<UserTask> GetTaskByVersion(int version);
        Task<UserTask> GetTaskByTitle(string title);
        Task<UserTask> GetTaskByDate(string date);
        Task<string> GetLastUpdaterNameById(int id);
        Task<string> GetFirstUpdaterNameById(int id);
    }
}
