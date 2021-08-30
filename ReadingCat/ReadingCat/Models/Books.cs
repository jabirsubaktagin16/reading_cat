
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ReadingCat.Models
{
    public class Books
    {
        public Books()
        {
            chapters = new List<Chapters>();
            currentChapter = new Chapters();
            comments = new List<Comment>();
            currentComment = new Comment();
        }
        public int bookId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        [Display(Name = "Book Name")]
        public string bookName { get; set; }
        public int userId { get; set; }
        public int rating { get; set; }
        public string bookCover { get; set; }
        public int inLibrary { get; set; }
        public int addToLibrary { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 50)]
        [Display(Name = "Description")]
        public string summary { get; set; }
        public string author { get; set; }

        
        [Required]
        [Display(Name = "Genre")]
        public string tag { get; set; }
        public int readCount { get; set; }
       // public int reviewing { get; set; }
        public Chapters currentChapter { get; set; }
        public Comment currentComment { get; set; }
        public List<Chapters> chapters { get; set; }
        public List<Comment> comments { get; set; }
    }
}