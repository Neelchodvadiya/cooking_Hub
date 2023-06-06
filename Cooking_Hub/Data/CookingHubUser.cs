using Microsoft.AspNetCore.Identity;

namespace Cooking_Hub.Data
{
    public class CookingHubUser: IdentityUser
    {
        public bool? IsActive { get; set; }
        public string? Gender { get; set; }
        public string? ImageFilePath { get; set; }
        public  string? FirstName { get; set; }
        public string? LastName { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
