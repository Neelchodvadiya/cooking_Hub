using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class BlogLike
{
    public string LikeId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string BlogId { get; set; } = null!;

    public DateTime? LikedAt { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
