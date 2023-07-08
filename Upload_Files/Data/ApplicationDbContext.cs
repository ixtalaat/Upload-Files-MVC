using Microsoft.EntityFrameworkCore;
using Upload_Files.Models;

namespace Upload_Files.Data
{
	public class ApplicationDbContext : DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<UploadedFile>  UploadedFiles { get; set; }
    }
}
