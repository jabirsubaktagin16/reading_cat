
using System.Collections.Generic;
using ReadingCat.Models;

namespace ReadingCat.ViewModel
{
    public class BooksAndDatabase
    {
        public BooksAndDatabase()
        {
            databaseModel = new DatabaseModel();
            listOfBooks = new List<List<Books>>();
            recommendation = new List<List<Books>>();
            tagList = new List<Tags>();
            followRecommendation = new List<User>();
            initializeList();
        }
        public DatabaseModel databaseModel { get; set; }

        public List<List<Books>> listOfBooks { get; set; }
        public List<Books> library { get; set; }
        public List<Books> publishedStories { get; set; }
        public List<List<Books>> recommendation { get; set; }
        public List<User> followRecommendation { get; set; }

        public List<Tags> tagList { get; set; }
        private void initializeList()
        {
            library = new List<Books>();
            publishedStories = new List<Books>();
           

            for(int i=0;i<3;i++)
            {
                //unapprovedListOfBooks[i] = new List<Books>();
                listOfBooks.Add(new List<Books>());
            }

            listOfBooks[0] = library;
            listOfBooks[1] = publishedStories;
            
        }

    }
}