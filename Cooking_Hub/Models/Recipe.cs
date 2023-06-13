using System;
using System.Collections.Generic;

namespace Cooking_Hub.Models;

public partial class Recipe
{
    public string RecipeId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string CategoryId { get; set; } = null!;

    public string? RecipeTitle { get; set; }

    public string CuisineId { get; set; } = null!;

    public string? RecipeshortDescription { get; set; }

    public string? RecipeDescription { get; set; }

    public TimeSpan? PreparationTime { get; set; }

    public TimeSpan? CookingTime { get; set; }

    public int? Serving { get; set; }

    public int? Views { get; set; }

    public string? Ingridients { get; set; }

    public string? Nutrition { get; set; }

    public string? RecipeImage { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Cuisine Cuisine { get; set; } = null!;

    public virtual ICollection<RecipeLike> RecipeLikes { get; set; } = new List<RecipeLike>();

    public virtual ICollection<RecipeReview> RecipeReviews { get; set; } = new List<RecipeReview>();

    public virtual AspNetUser User { get; set; } = null!;
}
