using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APLTest.Data.Models
{
    public class FileUpload
    {
        public int Id { get; set; }
        public int TotalSize { get; set; }
        public Guid UniqueReference { get; set; }
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Location { get; set; }
        public DateTime UploadedAt { get; set; }
        public StorageType StorageType{ get; set; }
        public Dictionary<string, string> AdditionalInfo { get; set; }
    }
}
