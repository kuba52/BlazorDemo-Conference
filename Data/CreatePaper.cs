using System.ComponentModel.DataAnnotations;

namespace Conference.Data;

public sealed class CreatePaper
{
    [Required]
    [StringLength(1023)]
    public string? Name { get; set; }

    [Required]
    [StringLength(1023)]
    public string? Classification { get; set; }

    public IEnumerable<int> AuthorIds { get; set; } = Enumerable.Empty<int>();
}