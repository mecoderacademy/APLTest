using APLTest.Data;
using APLTest.Data.Models;
using APLTest.Services.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace APLTest.Services
{
    public class AzureFileService : IFileService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureFileService> _logger;
        private readonly string _containerName;
        private readonly FileStorageContext _fileStorageContext;

        public AzureFileService(FileStorageContext fileStorageContext,IConfiguration configuration, ILogger<AzureFileService> logger)
        {
            _configuration = configuration;
            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AzureBlobStorageContainer"];
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
                if (data == null || data.Length < 1)
                {
                    return new FileUpload()
                    {
                        AdditionalInfo = { { "badFile", Constants.No_File_Found } }
                    };
                }

                var container = _blobServiceClient.GetBlobContainerClient(_containerName);
                var uploadClient = container.GetBlobClient(fileName);

                using (var stream = new MemoryStream(data))
                {
                    var blobInfo = await uploadClient.UploadAsync(stream);
                    var response = blobInfo.GetRawResponse();
                    if (response.IsError) {
                        return new FileUpload()
                        {
                            AdditionalInfo = { { "badFile", Constants.Error_uploading_file} }
                        };
                    }
                }

                string url = uploadClient?.Uri?.ToString() ?? string.Empty;
                var addtionalInfo = new Dictionary<string,string>{ { "uploadSuccessfull", Constants.File_Uploaded_Successful } };
                var uploadedAt = DateTime.UtcNow;
                var totalSize = data.Length;
                var fileTypeObj = fileType;
                var storageType = StorageType.cloud;

                return new FileUpload()
                {
                    Location = url,
                    AdditionalInfo = addtionalInfo,
                    Name = fileName ?? string.Empty,
                    UploadedAt = uploadedAt,
                    TotalSize = totalSize,
                    FileType = fileTypeObj,
                    StorageType = storageType,                   
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new FileUpload()
                {
                    AdditionalInfo = new Dictionary<string, string> { { "uploadFailed", ex.ToString() } }
                };
            }
        }
    }
}