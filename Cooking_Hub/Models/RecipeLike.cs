using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class RecipeLike
{
    public string RecipeLikeId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string RecipeId { get; set; } = null!;

    public DateTime LikedAt { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
