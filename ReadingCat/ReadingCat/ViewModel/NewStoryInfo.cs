
using System.Collections.Generic;
using System.Web;
using ReadingCat.Models;

namespace ReadingCat.ViewModel
{
    public class NewStoryInfo
    {
        public NewStoryInfo()
        {
            books = new Books();
            tags = new Tags();
            file = new List<HttpPostedFileBase>();
            listOfTags = new List<string>();
        }

       public Books books { get; set; }
        public Tags tags { get; set; }
        public List<string> listOfTags { get; set; }
        public List<HttpPostedFileBase> file { get; set; }
    }
}