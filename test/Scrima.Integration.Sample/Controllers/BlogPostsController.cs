using System;
using System.Linq.Expressions;
using Scrima.Integration.Sample.Data;
using Scrima.Integration.Sample.Models;

namespace Scrima.Integration.Sample.Controllers;

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
