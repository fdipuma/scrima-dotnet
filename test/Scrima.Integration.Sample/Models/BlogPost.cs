using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scrima.Integration.Sample.Models;

public class BlogPost
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string? Title { get; set; }
        
    public string? Text { get; set; }

    [ForeignKey(nameof(Blog))] public int BlogId { get; set; }

    public Blog Blog { get; set; }
}
