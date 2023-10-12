using APLTest.Data;
using APLTest.Data.Models;
using APLTest.Services.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace APLTest.Services
{
    public class LocalFileService : IFileService
    {
        private readonly string _locaPath;
        private readonly ILogger<LocalFileService> _logger;
        private readonly FileStorageContext _fileStorageContext;
        public LocalFileService(ILogger<LocalFileService> logger, FileStorageContext fileStorageContext)
        {
            _locaPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _logger = logger;
           _fileStorageContext = fileStorageContext;
    }

        public async Task<FileUpload> StoreImageDetailsAsync(FileUpload fileUpload)
        {
           await _fileStorageContext.FileUploads.AddAsync(fileUpload);
            await _fileStorageContext.SaveChangesAsync();
             return _fileStorageContext?.FileUploads?.SingleOrDefault(x => x.UniqueReference == fileUpload.UniqueReference);
        }

        public async Task<FileUpload> UploadFile(byte[] data, string fileName, string fileType)
        {
            try
            {
                var directory = $"{_locaPath}/uploads";
                var path = Path.Combine(directory, fileName);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(path))
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await stream.WriteAsync(data, 0, data.Length);
                    }
                }
                return new FileUpload()
                {
                    Location = path,
                    UniqueReference = Guid.NewGuid(),
                    AdditionalInfo = { { "uploadSuccessfull", Constants.File_Uploaded_Successful } },
                    Name = fileName,
                    UploadedAt = DateTime.UtcNow,
                    TotalSize = data.Length,
                    FileType = fileType,
                    StorageType = StorageType.local
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return new FileUpload()
                {
                    AdditionalInfo = new Dictionary<string, string> { { "uploadFailed", Constants.No_File_Found } }
                };
            }
        }

       
    }
}