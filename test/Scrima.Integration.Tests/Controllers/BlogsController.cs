using System;
using System.Linq.Expressions;
using Scrima.Integration.Tests.Data;
using Scrima.Integration.Tests.Models;

namespace Scrima.Integration.Tests.Controllers
{
    public class BlogsController : TestControllerBase<Blog>
    {
        public BlogsController(BlogDbContext context)
            : base(context.Blogs)
        {
        }

        protected override Expression<Func<Blog, string, bool>> GetSearchPredicate()
        {
            return (blog, searchText) => (blog.Description != null && blog.Description.Contains(searchText))
                                         || (blog.Name != null && blog.Name.Contains(searchText));
        }
    }
}
