using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class BlogComment
{
    public string CommentId { get; set; } = null!;

    public string? ParentId { get; set; }

    public string UserId { get; set; } = null!;

    public string BlogId { get; set; } = null!;

    public string? BcommentContents { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Blog Blog { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
