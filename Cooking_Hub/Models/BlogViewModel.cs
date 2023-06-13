namespace Cooking_Hub.Models
{
    public class BlogViewModel
    {
        public string BlogId { get; set; }
        public string BlogTitle { get; set; }
        public string BlogShortDescription { get; set; }
        public string BlogImage { get; set; }
        public bool? BlogIsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }
    }
}
