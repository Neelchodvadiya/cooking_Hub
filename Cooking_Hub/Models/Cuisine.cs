using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class Cuisine
{
    public string CuisineId { get; set; } = null!;

    public string CuisineName { get; set; } = null!;

    public string? CuisineImage { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
