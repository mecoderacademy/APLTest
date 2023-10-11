using APLTest.Data;
using APLTest.Services;
using APLTest.Services.Interfaces;
using APLTest.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace APLTest.Test.Controllers
{
    public class FileUploadControllerTest
    {
        ILogger<FileUploadController> _loggerController;
        ILogger<LocalFileService> _loggerLocal;
        FileUploadController fileUploadController;
        FileStorageContext fileStorageContext;
        IFileService _fileService;
        [SetUp]
        public void Setup()
        {
            fileUploadController = new FileUploadController(_loggerController, _fileService);
            _fileService = new LocalFileService(_loggerLocal, fileStorageContext);

        }

        [Test]
        public async Task PostImageUpload_NullFileToUpload_ReturnsBadRequestAsync()
        {
            IFormFile fileToUpload = null; //act

            var result = await fileUploadController.PostImageUploadAsync(fileToUpload) as BadRequestObjectResult; //arrage

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.That(result?.Value, Is.EqualTo(Constants.No_File_Found)); //assert

        }
        [Test]
        public async Task PostImageUpload_FileIsEmpty_ReturnsBadRequestAsync()
        {
            IFormFile fileToUpload = new FormFile(null, 0, 0, null, null);

            var result = await fileUploadController.PostImageUploadAsync(fileToUpload) as BadRequestObjectResult;

            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.That(result?.Value, Is.EqualTo(Constants.No_File_Found));

        }
        [Test]
        public async Task PostImageUpload_MockFormFileImage_ReturnsBadRequestIfNotImageAsync()
        {
           
            string testUploadFileName = "somefile.txt";

            var fileToUpload = createFormFileFromLocalFile(testUploadFileName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "application/text");
            var result = await fileUploadController.PostImageUploadAsync(fileToUpload) as BadRequestObjectResult;


            Assert.That(result?.StatusCode, Is.EqualTo(400));
            Assert.That(result?.Value, Is.EqualTo(Constants.Wrong_Image_Type));

        }

        [Test]
        public async Task PostImageUpload_MockFormFileImage_ReturnsCreatedWithUrlAsync()
        {
           
            string testUploadFileName = "testimage.jpeg";

            var fileToUpload = createFormFileFromLocalFile(testUploadFileName, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Image/jpeg");
            var result = await fileUploadController.PostImageUploadAsync(fileToUpload) as CreatedResult;


            Assert.That(result?.StatusCode, Is.EqualTo(201));
            Assert.That(result?.Value, Is.EqualTo(Constants.File_Uploaded_Successful));

        }
        private IFormFile createFormFileFromLocalFile(string fileName, string filePath, string contentType)
        {
            var path = Path.Combine(filePath, fileName);

            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                {
                    return new FormFile(stream, 0, stream.Length, "someName", fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType = contentType
                    };
                }
            }
            return null;
        }
    }
}