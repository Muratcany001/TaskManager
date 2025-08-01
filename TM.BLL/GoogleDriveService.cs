using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TM.BLL
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;

        public GoogleDriveService()
        {
            GoogleCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.Scope.Drive);
            }

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "TaskManager"
            });
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = file.FileName,
                Parents = new List<string> { "TaskManager" } // Not: Gerçek klasör ID’si olmalı
            };

            using var stream = file.OpenReadStream();

            var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
            request.Fields = "id";
            var uploadResult = await request.UploadAsync();

            if (uploadResult.Status == UploadStatus.Completed)
            {
                string fileId = request.ResponseBody.Id;
                return $"https://drive.google.com/file/d/{fileId}/view";
            }

            throw new Exception("Google Drive yükleme başarısız.");
        }

        public async Task<string> DeleteFileAsync(string fileId)
        {
            await _driveService.Files.Delete(fileId).ExecuteAsync();
            return fileId; // İstersen string.Empty ya da özel mesaj dönebilirsin
        }

        public async Task<string> UpdateFileAsync(string fileId, IFormFile newFile)
        {
            var fileMetadata = new Google.Apis.Drive.v3.Data.File
            {
                Name = newFile.FileName
            };

            using var stream = newFile.OpenReadStream();

            var updateRequest = _driveService.Files.Update(fileMetadata, fileId, stream, newFile.ContentType);
            updateRequest.Fields = "id";

            var uploadResult = await updateRequest.UploadAsync();

            if (uploadResult.Status == UploadStatus.Completed)
            {
                return $"https://drive.google.com/file/d/{fileId}/view";
            }

            throw new Exception("Google Drive dosya güncelleme başarısız.");
        }
    }
}
