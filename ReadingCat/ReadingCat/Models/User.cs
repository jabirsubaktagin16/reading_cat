using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace ReadingCat.Models
{

    public class User
    {
        public User()
        {
            File = new List<HttpPostedFileBase>();

        }
        public int userid { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "username")]
        public string username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email ID")]
        [Display(Name = "Email Address")]
        public string useremail { get; set; }
        public string bio { get; set; }

        
        [Required]
        [StringLength(10, MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "password")]
        public string password { get; set; }

        
        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "The password and confirm password do not match")]
        public string confirmPassword { get; set; }
        public int isAdmin { get; set; }
        public List<HttpPostedFileBase> File { get; set; }
        public string paths { get; set; }
    }


}