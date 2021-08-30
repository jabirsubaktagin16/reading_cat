
using System.ComponentModel.DataAnnotations;

namespace ReadingCat.Models
{
    public class LoginModel
    {
       [Required]
       [Display(Name = "username")]
       public string username { get; set; }

        [Required]
        [Display(Name = "password")]
        public  string password { get; set; }
        public int userid { get; set; }
        public string path { get; set; }

        public int isAdmin { get; set; }
        public string bio { get; set; }
        public int followerNum { get; set; }
        public int totalPublished { get; set; }
        public int totalViews { get; set; }
        public int isFollowing { get; set; }

    }
}