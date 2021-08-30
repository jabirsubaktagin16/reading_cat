
using System.Collections.Generic;
using ReadingCat.Models;
namespace ReadingCat.ViewModel
{
    public class UnapprovedChapters
    {
        public UnapprovedChapters()
        {
            unapprovedListOfBooks = new List<Books>();
        }

        public List<Books> unapprovedListOfBooks { get; set; }
        
    }
}