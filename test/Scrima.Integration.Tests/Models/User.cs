using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scrima.Integration.Tests.Models;

public class User : Person
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
        
    public string Username { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateOnly RegistrationDate { get; set; }
        
    public UserType Type { get; set; }
    public UserType SecondaryType { get; set; }

    public string EMail { get; set; }

    [InverseProperty(nameof(Blog.Owner))]
    public ICollection<Blog> Blogs { get; set; }

    public double Engagement { get; set; }
    public decimal PayedAmout { get; set; }
    public string DomainId { get; set; } = string.Empty;
        
}
