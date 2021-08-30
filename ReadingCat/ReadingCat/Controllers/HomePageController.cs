using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using ReadingCat.Models;
using ReadingCat.ViewModel;

namespace ReadingCat.Controllers
{
    public class HomePageController : Controller
    {

        BooksAndDatabase booksAndDatabase = new BooksAndDatabase();
        // GET: HomePage
        public ActionResult HomePage()
        {
            var exemploList = new SelectList(new[] { "Exemplo1:", "Exemplo2", "Exemplo3" });
            ViewBag.ExemploList = exemploList;
            GetRecommendation();
            GetNewRelease();
            if (Session["Id"] != null)
            {
                GetPeopleRecoomendation();
            }
            return View(booksAndDatabase);
        }

        private void GetNewRelease()
        {
            string query = "SELECT *FROM BOOKS";
            DatabaseModel databaseModel = new DatabaseModel();
            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                int size = dataSet.Tables[0].Rows.Count;
                size -= 1;
                for (int i = size; i >= 10; i--)
                {
                    Books books = new Books();
                    books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    books.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    books.bookCover = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                    booksAndDatabase.listOfBooks[1].Add(books);
                }
            }
        }

        private void GetRecommendation()
        {
            string query = "";
            if (Session["Id"] != null)
            {
                 query = "SELECT COUNT(READLOG.BOOKID), READLOG.BOOKID, BOOKS.BookName, BOOKS.BookCover, BOOKS.UserId, Books.Rating, BOOKS.BookCover, BOOKTAGS.TAGID FROM READLOG LEFT JOIN BOOKTAGS ON ReadLog.BookId = BookTags.BookId LEFT JOIN Books ON ReadLog.BookId = BOOKS.BookID GROUP BY READLOG.BookId, BookTags.TAGID, BOOKS.BookName, BOOKS.BookCover, BOOKS.Rating, BOOKS.UserId EXCEPT SELECT COUNT(READLOG.BOOKID), READLOG.BOOKID, BOOKS.BookName, BOOKS.BookCover, BOOKS.UserId, Books.Rating, BOOKS.BookCover, BOOKTAGS.TAGID FROM READLOG LEFT JOIN BOOKTAGS ON ReadLog.BookId = BookTags.BookId LEFT JOIN Books ON ReadLog.BookId = BOOKS.BookID WHERE ReadLog.UserId = " + (int)System.Web.HttpContext.Current.Session["Id"] + " GROUP BY READLOG.BookId, BookTags.TAGID, BOOKS.BookName, BOOKS.BookCover, BOOKS.Rating, BOOKS.UserId";
            }
            else
            {
                query = "SELECT COUNT(READLOG.BOOKID), READLOG.BOOKID, BOOKS.BookName, BOOKS.BookCover, BOOKS.UserId, Books.Rating, BOOKS.BookCover, BOOKTAGS.TAGID FROM READLOG LEFT JOIN BOOKTAGS ON ReadLog.BookId = BookTags.BookId LEFT JOIN Books ON ReadLog.BookId = BOOKS.BookID GROUP BY READLOG.BookId, BookTags.TAGID, BOOKS.BookName, BOOKS.BookCover, BOOKS.Rating, BOOKS.UserId";
            }
                DatabaseModel databaseModel = new DatabaseModel();
            DataSet dataSet = databaseModel.selectFunction(query);

            for (int i = (dataSet.Tables[0].Rows.Count - 1); i >= 0; i--)
            {

                Books book = new Books();
                book.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[1]);
                book.bookName = dataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                book.bookCover = dataSet.Tables[0].Rows[i].ItemArray[3].ToString();
                book.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[4]);
                // book.rating = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[4]);
                booksAndDatabase.listOfBooks[2].Add(book);
            }
        }

        private void GetPeopleRecoomendation()
        {
            int userid = (int)System.Web.HttpContext.Current.Session["Id"];
            List<int> listOfFollower = new List<int>();

            GetFollowList(listOfFollower, userid);
            CreateFollowRecommendation(listOfFollower, userid);
        }

        private void GetFollowList(List<int> listOfFollower, int userid)
        {
            string query = "SELECT FOLLOWING FROM FOLLOW WHERE FOLLOWER = " + userid;
            DataSet dataSet = new DataSet();
            DatabaseModel database = new DatabaseModel();
            dataSet = database.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    listOfFollower.Add(Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]));
                }
            }
        }


        private void CreateFollowRecommendation(List<int> listOfFollower, int userid)
        {
            if (listOfFollower.Count > 0)
            {
                List<int> uniqueID = new List<int>();
                for (int i = 0; i < listOfFollower.Count; i++)
                {
                    int following = listOfFollower[i];
                    uniqueID.Add(following);
                    string query1 = "SELECT USERID, PHOTO, USERNAME FROM USERS WHERE USERID IN (SELECT FOLLOWING FROM FOLLOW WHERE FOLLOWER IN (SELECT FOLLOWER FROM FOLLOW WHERE FOLLOWING = " + following + ") EXCEPT (SELECT FOLLOWING FROM FOLLOW WHERE FOLLOWING = " + following + " OR FOLLOWER = " + userid + " ))";
                    DataSet dataSet1 = new DataSet();
                    DatabaseModel database1 = new DatabaseModel();
                    dataSet1 = database1.selectFunction(query1);
                    if (dataSet1.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dataSet1.Tables[0].Rows.Count; j++)
                        {
                            int followingId = Convert.ToInt32(dataSet1.Tables[0].Rows[j].ItemArray[0]);
                            if (!uniqueID.Contains(followingId))
                            {
                                User user = new User();
                                user.userid = followingId;
                                string img = dataSet1.Tables[0].Rows[j].ItemArray[1].ToString();
                                if (string.IsNullOrEmpty(img))
                                {
                                    user.paths = "~/images/profile.png";
                                }
                                else
                                {
                                    user.paths = dataSet1.Tables[0].Rows[j].ItemArray[1].ToString();
                                }
                                user.username = dataSet1.Tables[0].Rows[j].ItemArray[2].ToString();
                                booksAndDatabase.followRecommendation.Add(user);
                                uniqueID.Add(followingId);
                            }

                        }
                    }
                }

                string query = "SELECT COUNT(FOLLOWING), USERID, PHOTO, USERNAME FROM FOLLOW LEFT JOIN USERS ON USERS.userid = Follow.following GROUP BY FOLLOWING, USERID, PHOTO, USERNAME ORDER BY COUNT(FOLLOWING) DESC";
                DataSet dataSet = new DataSet();
                DatabaseModel database = new DatabaseModel();
                dataSet = database.selectFunction(query);
                if (dataSet.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dataSet.Tables[0].Rows.Count; j++)
                    {
                        int followingId = Convert.ToInt32(dataSet.Tables[0].Rows[j].ItemArray[1]);
                        if (!uniqueID.Contains(followingId))
                        {
                            User user = new User();
                            user.userid = followingId;
                            string img = dataSet.Tables[0].Rows[j].ItemArray[2].ToString();
                            if (string.IsNullOrEmpty(img))
                            {
                                user.paths = "~/images/profile.png";
                            }
                            else
                            {
                                user.paths = img;
                            }
                            user.username = dataSet.Tables[0].Rows[j].ItemArray[3].ToString();
                            booksAndDatabase.followRecommendation.Add(user);
                            uniqueID.Add(followingId);
                        }

                    }
                }
            }
            else
            {
                string query2 = "SELECT COUNT(FOLLOWING), USERID, PHOTO, USERNAME FROM FOLLOW LEFT JOIN USERS ON USERS.userid = Follow.following GROUP BY FOLLOWING, USERID, PHOTO, USERNAME ORDER BY COUNT(FOLLOWING) DESC";
                DataSet dataSet2 = new DataSet();
                DatabaseModel database2 = new DatabaseModel();
                dataSet2 = database2.selectFunction(query2);
                if (dataSet2.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dataSet2.Tables[0].Rows.Count; j++)
                    {
                        int followingId = Convert.ToInt32(dataSet2.Tables[0].Rows[j].ItemArray[1]);
                        if (followingId != userid)
                        {
                            User user = new User();
                            user.userid = followingId;
                            string img = dataSet2.Tables[0].Rows[j].ItemArray[2].ToString();
                            if (string.IsNullOrEmpty(img))
                            {
                                user.paths = "~/images/profile.png";
                            }
                            else
                            {
                                user.paths = img;
                                user.paths = img;
                            }
                            user.username = dataSet2.Tables[0].Rows[j].ItemArray[3].ToString();
                            booksAndDatabase.followRecommendation.Add(user);

                        }
                    }
                }
            }
            }
        }
    }
