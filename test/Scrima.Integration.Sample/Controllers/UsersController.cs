using System;
using System.Linq.Expressions;
using Scrima.Integration.Sample.Data;
using Scrima.Integration.Sample.Models;

namespace Scrima.Integration.Sample.Controllers;

public class UsersController : TestControllerBase<User>
{
    public UsersController(BlogDbContext context)
        : base(context.Users)
    {
    }

    protected override Expression<Func<User, string, bool>> GetSearchPredicate()
    {
        return (blog, searchText) => (blog.Username != null && blog.Username.Contains(searchText))
                                     || (blog.FirstName != null && blog.FirstName.Contains(searchText))
                                     || (blog.LastName != null && blog.LastName.Contains(searchText));
    }
}
