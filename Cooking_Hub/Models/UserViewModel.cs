namespace Cooking_Hub.Models
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool? IsActive { get; set; }
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ImageFilePath { get; set; }
        public int TotalCreatedRecipes { get; set; }
		public int TotalLikedRecipes { get; set; }
		public int TotalReviewedRecipes { get; set; }
	}
}
