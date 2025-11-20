using System;
using Microsoft.EntityFrameworkCore;
using Scrima.Integration.Sample.Models;

namespace Scrima.Integration.Sample.Data;

public sealed class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogPost> BlogPosts { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<DateOnly>()
            .HaveColumnType("date");
    }
}
