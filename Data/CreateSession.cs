using System.ComponentModel.DataAnnotations;

namespace Conference.Data;

public class CreateSession
{
    [Required]
    public DateTime When { get; set; }

    [Required]
    public int? ChairId { get; set; }
}