using Microsoft.EntityFrameworkCore;
using Scrima.Integration.Tests.Models;

namespace Scrima.Integration.Tests.Data;

public sealed class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }
}
