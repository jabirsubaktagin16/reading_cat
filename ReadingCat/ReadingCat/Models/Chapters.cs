
using System.ComponentModel.DataAnnotations;

namespace ReadingCat.Models
{
    public class Chapters
    {
        public int chapterId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 1)]
        public string chapterName { get; set; }

        [Required]
        [StringLength(int.MaxValue, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 300)]
        public string chatpterText { get; set; }
        public int approved { get; set; }

        public int bookId { get; set; }

    }
}