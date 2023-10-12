using APLTest.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace APLTest.Data
{
    public class FileStorageContext : DbContext
    {
        //public FileStorageContext() { }
        public FileStorageContext(DbContextOptions<FileStorageContext> dbContextOptions) : base(dbContextOptions) 
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileUpload>().HasKey(x => x.Id);
            modelBuilder.Entity<FileUpload>().Ignore(x => x.AdditionalInfo);
            

        }

        public DbSet<FileUpload> FileUploads { get; set; }
    }

}