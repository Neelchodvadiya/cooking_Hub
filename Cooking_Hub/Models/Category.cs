using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string CategoryName { get; set; } = null!;

    public bool? CategoryIsActive { get; set; }

    public string? CategoryImage { get; set; }
  

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
