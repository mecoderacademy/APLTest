using APLTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APLTest.Services.Interfaces
{
    public interface IFileService
    {
        public Task<FileUpload> UploadFile(byte[] data, string fileName, string fileType);
        public FileUpload StoreImageDetails(FileUpload fileUpload);
    }
}
