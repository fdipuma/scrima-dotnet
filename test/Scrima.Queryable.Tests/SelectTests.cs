using System.Collections.Generic;
using System.Linq;
using Scrima.Core;
using Scrima.Core.Exceptions;
using Scrima.Core.Model;
using Scrima.Core.Query;
using Shouldly;
using Xunit;

namespace Scrima.Queryable.Tests;

public class SelectTests
{
    private static readonly EdmComplexType PostType = (EdmComplexType)new EdmTypeProvider().GetByClrType(typeof(Post));

    private readonly IQueryable<Post> _posts = new[]
    {
        new Post { Id = 1, Title = "First", Blog = new Blog { Name = "Scrima", Description = "OData for .NET" } },
        new Post { Id = 2, Title = "Second" }
    }.AsQueryable();

    [Fact]
    public void Should_PopulateOnlySelectedProperties()
    {
        var posts = _posts.Select(SelectPaths("title")).ToList();

        posts.Select(p => p.Title).ShouldBe(["First", "Second"]);
        posts.ShouldAllBe(p => p.Id == 0 && p.Blog == null);
    }

    [Fact]
    public void Should_ProjectNestedProperties_When_PathIsSelected()
    {
        var posts = _posts.Select(SelectPaths("blog/name")).ToList();

        posts[0].Title.ShouldBeNull();
        posts[0].Blog.Name.ShouldBe("Scrima");
        posts[0].Blog.Description.ShouldBeNull();
        posts[1].Blog.ShouldBeNull();
    }

    [Fact]
    public void Should_SelectWholeObject_When_ObjectAndNestedPathAreSelected()
    {
        var post = _posts.Select(SelectPaths("blog/name", "blog")).First();

        post.Blog.Name.ShouldBe("Scrima");
        post.Blog.Description.ShouldBe("OData for .NET");
    }

    [Fact]
    public void Should_ReturnSourceUnchanged_When_AllPropertiesAreSelected()
    {
        _posts.Select(SelectQueryOption.All).ShouldBeSameAs(_posts);
    }

    [Fact]
    public void Should_Throw_When_SelectedPropertyCannotBeSet()
    {
        Should.Throw<QueryOptionsValidationException>(() => _posts.Select(SelectPaths("titleLength")));
    }

    [Fact]
    public void Should_SelectProperties_After_Paging()
    {
        var queryOptions = new QueryOptions(PostType, new FilterQueryOption(null), new OrderByQueryOption([]),
            search: null, skip: 1, skipToken: null, top: null, showCount: true, select: SelectPaths("title"));

        var result = _posts.ToQueryResult(queryOptions);

        result.Count.ShouldBe(2);
        var post = result.Results.ShouldHaveSingleItem();
        post.Title.ShouldBe("Second");
        post.Id.ShouldBe(0);
    }

    [Fact]
    public void Should_ProjectNestedProperties_When_PathGoesThroughStruct()
    {
        var posts = new[] { new Post { Stats = new Stats { Views = 10, Likes = 3 } } }.AsQueryable();

        var post = posts.Select(SelectPaths("stats/views")).Single();

        post.Stats.Views.ShouldBe(10);
        post.Stats.Likes.ShouldBe(0);
    }

    [Fact]
    public void Should_SelectDerivedProperty_When_PropertyIsHidden()
    {
        var specialType = (EdmComplexType)new EdmTypeProvider().GetByClrType(typeof(SpecialPost));
        var posts = new[] { new SpecialPost { Id = 1, Title = "Special" } }.AsQueryable();

        var post = posts.Select(SelectPaths(specialType, "title")).Single();

        post.Title.ShouldBe("Special");
        post.Id.ShouldBe(0);
    }

    [Fact]
    public void Should_Throw_When_TypeHasNoParameterlessConstructor()
    {
        var dtoType = (EdmComplexType)new EdmTypeProvider().GetByClrType(typeof(PostDto));
        var posts = new[] { new PostDto("First") }.AsQueryable();

        Should.Throw<QueryOptionsValidationException>(() => posts.Select(SelectPaths(dtoType, "title")));
    }

    private static SelectQueryOption SelectPaths(params string[] paths) => SelectPaths(PostType, paths);

    private static SelectQueryOption SelectPaths(EdmComplexType rootType, params string[] paths) =>
        new(paths.Select(path => new SelectProperty(ResolvePath(rootType, path))));

    private static IEnumerable<EdmProperty> ResolvePath(EdmComplexType type, string path)
    {
        foreach (var name in path.Split('/'))
        {
            var property = type.GetProperty(name);
            yield return property;
            type = property.PropertyType as EdmComplexType;
        }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int TitleLength => Title.Length;
        public Blog Blog { get; set; }
        public Stats Stats { get; set; }
    }

    public class SpecialPost : Post
    {
        public new string Title { get; set; }
    }

    public record PostDto(string Title);

    public struct Stats
    {
        public int Views { get; set; }
        public int Likes { get; set; }
    }

    public class Blog
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
