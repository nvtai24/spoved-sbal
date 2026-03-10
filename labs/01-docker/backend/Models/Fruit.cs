using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Fruit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public int? Sweetness { get; set; }

    public DateTime? CreatedAt { get; set; }
}
