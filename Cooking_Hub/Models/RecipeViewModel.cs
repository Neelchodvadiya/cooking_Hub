namespace Cooking_Hub.Models
{
	public class RecipeViewModel
	{
		public string RecipeId { get; set; }
		public string UserName { get; set; }
		public string RecipeTitle { get; set; }
		public string RecipeShortDescription { get; set; }
		public string? RecipeImage { get; set; }
		public int? Views { get; set; }
		public bool? IsActive { get; set; }
		public DateTime? CreatedAt { get; set; }
		public int TotalComments { get; set; }
		public int TotalLikes { get; set; }
        public string? Categoryname { get; set; }
    }
}
