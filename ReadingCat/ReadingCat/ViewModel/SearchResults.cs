using System.Collections.Generic;
using ReadingCat.Models;
namespace ReadingCat.ViewModel
{
    public class SearchResults
    {
        public SearchResults()
        {
            searchByTag = new List<Books>();
            searchByName = new List<Books>();
            searchByUserName = new List<User>();
        }
        public List<Books> searchByTag { get; set; }
        public List<Books> searchByName { get; set; }
        public List<User> searchByUserName { get; set; }
    }
}