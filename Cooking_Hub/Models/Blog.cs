using Cooking_Hub.Data;
using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class Blog
{
    public string BlogId { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? BlogTitle { get; set; }

    public string? BlogshortDescription { get; set; }

    public string? BlogContents { get; set; }

    public string? BlogImage { get; set; }

    public bool? BlogIsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    public virtual ICollection<BlogLike> BlogLikes { get; set; } = new List<BlogLike>();

    public virtual Category Category { get; set; } = null!;

    public virtual AspNetUser User { get; set; } = null!;
}
