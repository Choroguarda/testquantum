using Microsoft.EntityFrameworkCore;
using TestQuantumDocs.Models;

namespace TestQuantumDocs.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentPageIndex> DocumentPageIndexes { get; set; }
    }
}
