using System.ComponentModel.DataAnnotations;

namespace Conference.Data;

public sealed class CreateAuthor
{
    [Required]
    [StringLength(255)]
    public string? Name { get; set; }

    [Required]
    [StringLength(255)]
    public string? Surname { get; set; }
}