using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TM.BLL
{
    public interface IGoogleDriveService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<string> DeleteFileAsync(string fileId);
        Task<string> UpdateFileAsync(string fileId, IFormFile newFile);
    }
}
