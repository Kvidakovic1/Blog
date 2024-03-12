using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlogZavrsni.Models;

namespace BlogZavrsni.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BlogZavrsni.Models.Tag>? Tag { get; set; }
        public DbSet<BlogZavrsni.Models.Post>? Post { get; set; }
        public DbSet<PostTag> PostTag { get; set; }
    }
}
