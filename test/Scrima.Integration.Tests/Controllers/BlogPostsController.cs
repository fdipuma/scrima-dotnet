using System;
using System.Linq.Expressions;
using Scrima.Integration.Tests.Data;
using Scrima.Integration.Tests.Models;

namespace Scrima.Integration.Tests.Controllers;

public class BlogPostsController : TestControllerBase<BlogPost>
{
    public BlogPostsController(BlogDbContext context)
        : base(context.BlogPosts)
    {
    }

    protected override Expression<Func<BlogPost, string, bool>> GetSearchPredicate()
    {
        return (post, searchText) => (post.Text != null && post.Text.Contains(searchText))
                                     || (post.Title != null && post.Title.Contains(searchText));
    }
}
