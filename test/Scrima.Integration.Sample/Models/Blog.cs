using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scrima.Integration.Sample.Models;

public class Blog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    [ForeignKey(nameof(Owner))]
    public int OwnerId { get; set; }
    public User Owner { get; set; }

    [InverseProperty(nameof(BlogPost.Blog))]
    public ICollection<BlogPost> Posts { get; set; }
}
