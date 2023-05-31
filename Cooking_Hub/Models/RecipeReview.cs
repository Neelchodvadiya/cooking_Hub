using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class RecipeReview
{
    public string ReviewId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string RecipeId { get; set; } = null!;

    public int? Rating { get; set; }

    public string? ReviewContents { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
