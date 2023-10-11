using APLTest.Data;
using APLTest.Services.Interfaces;
using APLTest.Web;
using Microsoft.AspNetCore.Mvc;

namespace APLTest.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

        [HttpGet(Name = nameof(PostImageUploadAsync))]
        public async Task<IActionResult> PostImageUploadAsync(IFormFile file)
        {
            
            if (file == null || file.Length <= 0) return BadRequest(Constants.No_File_Found);
            if (file.ContentType!=Constants.Png_Mime || file.ContentType!= Constants.Jpeg_Mime) return BadRequest(Constants.Wrong_Image_Type);


            var fileUpload = await _fileService.UploadFile(await ConvertFormFileToBytes(file), file.FileName, file.ContentType);

            return Created(fileUpload.Location, Constants.File_Uploaded_Successful);
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