using APLTest.Data;
using APLTest.Services.Interfaces;
using APLTest.Web;
using Microsoft.AspNetCore.Mvc;

namespace APLTest.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class FileUploadController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<FileUploadController> _logger;
        private readonly IFileService _fileService;
        public FileUploadController(ILogger<FileUploadController> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }

        [HttpPost(Name = "FileUpload")]
        public async Task<IActionResult> PostImageUploadAsync(IFormFile fileToUpload)
        {
            
            if (fileToUpload == null || fileToUpload.Length <= 0) return BadRequest(Constants.No_File_Found);
            if (fileToUpload.ContentType!=Constants.Png_Mime && fileToUpload.ContentType!= Constants.Jpeg_Mime) return BadRequest(Constants.Wrong_Image_Type);


            var fileUpload = await _fileService.UploadFile(await ConvertFormFileToBytes(fileToUpload), fileToUpload.FileName, fileToUpload.ContentType);
            if (string.IsNullOrEmpty(fileUpload.Location)) return BadRequest(Constants.Error_uploading_file);
            await _fileService.StoreImageDetailsAsync(fileUpload);
            return Created(string.Empty, fileUpload);
        }

        private async Task<byte[]> ConvertFormFileToBytes(IFormFile formFile)
        {
            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                return stream.ToArray();
            }
        }
    }
}