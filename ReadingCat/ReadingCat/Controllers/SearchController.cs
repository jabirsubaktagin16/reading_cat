using System;
using System.Web.Mvc;
using ReadingCat.ViewModel;
using ReadingCat.Models;
using System.Data;

namespace ReadingCat.Controllers
{
    public class SearchController : Controller
    {
        SearchResults searchResults = new SearchResults();
        // GET: Search
        [HttpPost]
        public ActionResult Index(string id)
        {
            string searchString = id;
            searchString.Trim();
           
            string[] arrayOfString = searchString.Split(new char[0]);
            if (arrayOfString.Length == 1)
            {
                SearchByTag(searchString);
            }

            SearchByName(searchString);
            SearchByUserName(searchString);
            return View(searchResults);
        }

        private void SearchByTag(string searchByString)
        {
            String query = "SELECT *FROM BOOKS WHERE BOOKID IN (SELECT BOOKID FROM BOOKTAGS WHERE TAGID = (SELECT TAGID FROM TAGS WHERE TAGNAME = '" + searchByString + "'))";
            DatabaseModel databaseModel = new DatabaseModel();
            DataSet dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Books books = new Books();
                    books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    books.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    //books.rating = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[3]);
                    books.bookCover = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                    searchResults.searchByTag.Add(books);
                }
            }
        }

        private void SearchByName(string searchString)
        {
            string query = "SELECT *FROM BOOKS WHERE BOOKNAME LIKE '%" + searchString + "%'";
            DatabaseModel databaseModel = new DatabaseModel();
            DataSet dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Books books = new Books();
                    books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    books.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    //books.rating = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[3]);
                    books.bookCover = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                    searchResults.searchByName.Add(books);
                }
            }
        }

        private void SearchByUserName(string searchString)
        {
            string query = "SELECT USERID, USERNAME, PHOTO FROM USERS WHERE USERNAME LIKE '%" + searchString + "%'";
            DatabaseModel databaseModel = new DatabaseModel();
            DataSet dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    User user = new User();
                    user.userid = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    user.username = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    user.paths = dataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    searchResults.searchByUserName.Add(user);
                }
            }
        }
    }
}