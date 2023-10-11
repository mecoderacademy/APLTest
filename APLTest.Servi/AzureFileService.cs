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

            var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AzureBlobStorageContainer"];
            _logger = logger;
            _fileStorageContext = fileStorageContext;

        }

        public FileUpload StoreImageDetails(FileUpload fileUpload)
        {
            _fileStorageContext.FileUploads.AddAsync(fileUpload);
            _fileStorageContext.SaveChangesAsync();
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
                }

                return new FileUpload()
                {
                    Location = uploadClient.Uri.ToString(),
                    AdditionalInfo = { { "uploadSuccessfull", Constants.File_Uploaded_Successful } },
                    Name = fileName,
                    UploadedAt = DateTime.UtcNow,
                    TotalSize = data.Length,
                    FileType = fileType,
                    StorageType = StorageType.cloud
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return new FileUpload()
                {
                    AdditionalInfo = new Dictionary<string, string> { { "uploadFailed", Constants.No_File_Found } }
                };
            }
        }
    }
}