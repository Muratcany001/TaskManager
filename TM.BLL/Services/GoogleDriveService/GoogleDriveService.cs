using Azure.Core;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TM.BLL.Services.GoogleDriveService
{
    public class GoogleDriveService : IGoogleDriveService, IDisposable
    {
        private readonly DriveService _driveService;

        public GoogleDriveService()
        {
            try
            {

                var serviceAccountJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "credentials.json");

                if (!File.Exists(serviceAccountJsonPath))
                    throw new FileNotFoundException($"Service Account JSON bulunamadı: {serviceAccountJsonPath}");

                var credential = GoogleCredential.FromFile(serviceAccountJsonPath)
                    .CreateScoped(DriveService.Scope.Drive);

                _driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TaskManagerUploads"
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Google Drive servis başlatılamadı: {ex.Message}", ex);
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Dosya geçersiz veya boş");

                if (file.Length > 10 * 1024 * 1024)
                    throw new ArgumentException("Dosya boyutu çok büyük (max 10MB)");


                var folderId = await GetOrCreateFolderIdAsync("TaskManager");

                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = file.FileName,
                    Parents = new List<string> { folderId }
                };

                using var stream = file.OpenReadStream();

                var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id,name,webViewLink,webContentLink";
                request.SupportsAllDrives = true;
                var uploadResult = await request.UploadAsync();

                if (uploadResult.Status == UploadStatus.Completed)
                {
                    string fileId = request.ResponseBody.Id;
                    await MakeFilePublicAsync(fileId);
                    return $"https://drive.google.com/file/d/{fileId}/view";
                }
                else if (uploadResult.Status == UploadStatus.Failed)
                {
                    throw new Exception($"Google Drive yükleme başarısız: {uploadResult.Exception?.Message}");
                }
                else
                {
                    throw new Exception($"Beklenmeyen durum: {uploadResult.Status}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya yükleme hatası: {ex.Message}", ex);
            }
        }

        public async Task<string> DeleteFileAsync(string fileId)
        {
            try
            {
                await _driveService.Files.Delete(fileId).ExecuteAsync();
                return fileId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya silme hatası: {ex.Message}", ex);
            }
        }

        public async Task<string> UpdateFileAsync(string fileId, IFormFile newFile)
        {
            try
            {
                if (newFile == null || newFile.Length == 0)
                    throw new ArgumentException("Güncellenecek dosya geçersiz");

                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = newFile.FileName
                };

                using var stream = newFile.OpenReadStream();

                var updateRequest = _driveService.Files.Update(fileMetadata, fileId, stream, newFile.ContentType);
                updateRequest.Fields = "id,name,webViewLink";
                updateRequest.SupportsAllDrives = true;

                var uploadResult = await updateRequest.UploadAsync();

                if (uploadResult.Status == UploadStatus.Completed)
                {
                    return $"https://drive.google.com/file/d/{fileId}/view";
                }
                else
                {
                    throw new Exception($"Google Drive dosya güncelleme başarısız: {uploadResult.Exception?.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya güncelleme hatası: {ex.Message}", ex);
            }
        }

        private async Task<string> GetOrCreateFolderIdAsync(string folderName)
        {
            try
            {

                string existingFolderId = "1cavhhiWHxyr4MxIOa63UHc3aoO8Tkhmz";

                try
                {
                    var testRequest = _driveService.Files.Get(existingFolderId);
                    var testResult = await testRequest.ExecuteAsync();
                    return existingFolderId;
                }
                catch
                {

                }


                var listRequest = _driveService.Files.List();
                listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{folderName}' and trashed=false";
                listRequest.Fields = "files(id,name)";
                listRequest.SupportsAllDrives = true;

                var result = await listRequest.ExecuteAsync();
                var folder = result.Files.FirstOrDefault();

                if (folder != null)
                    return folder.Id;


                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder"
                };

                var createRequest = _driveService.Files.Create(fileMetadata);
                createRequest.Fields = "id";
                createRequest.SupportsAllDrives = true;

                var createdFolder = await createRequest.ExecuteAsync();
                return createdFolder.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Klasör oluşturma/bulma hatası: {ex.Message}", ex);
            }
        }

        private async Task MakeFilePublicAsync(string fileId)
        {
            try
            {
                var permission = new Google.Apis.Drive.v3.Data.Permission()
                {
                    Role = "reader",
                    Type = "anyone"
                };

                var request = _driveService.Permissions.Create(permission, fileId);
                await request.ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Dosyayı public yapma hatası: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _driveService?.Dispose();
        }
    }
}
