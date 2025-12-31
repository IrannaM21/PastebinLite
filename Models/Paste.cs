using System.ComponentModel.DataAnnotations;

namespace PastebinLite.Models;

public class Paste
{
    [Key]
    public string Id { get; set; } = default!;

    [Required]
    public string Content { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }

    public int? TtlSeconds { get; set; }

    public int? MaxViews { get; set; }

    public int Views { get; set; }
}
