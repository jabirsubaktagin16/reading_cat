using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using ReadingCat.Models;
using System.IO;
using ReadingCat.ViewModel;

//The Controller is responsible for viewing the profile of a user
//This corresponding View of this controller will view the library, published list and
//recommendation by the user's preferred tags
//User will also be able to navigate to the home page or edit his information here
namespace ReadingCat.Controllers
{
    public class ProfileController : Controller
    {
        //This controller uses an object of LoginAndDatabase model
        //which will contain information about the user that has logged in and the books of their
        //library and recommendatons
        //The reason we are not using Session for these information is because
        //Users will be able to view other user's pofiles and the guest user's information
        //will be viewed based on the LoginModel and BooksAndDatabase iformation of the guest user
        //Other options, which are private to a user, will be handled by the value of Session
        //For better understanding, let us assume the parameter ""id" of Profil method to be the
        //id of the guest user
        //a user, even if he is in his own profile, will be considered as guest user in this
        //case, where he is the host user as well
       
        LoginAndBookList loginAndBookList = new LoginAndBookList();
        
        //The following object is needed because LoginAndBookList class contains an object of
        //BooksAndDatabase. This object will hold the information of the book lists that will be
        //retieved from the database, such as Library, Published List and Recommendation
        BooksAndDatabase booksAndDatabase = new BooksAndDatabase();
        string pathName = "";
        Hashing hashing = new Hashing();
        //This method is responsible for loading the profile of the user/guest user
        [HttpGet]
        public ActionResult Profile(int id)
        {
            loginAndBookList.loginModel = new LoginModel();

            //Different components of the guest user's profile reqire different queries
            //which will be brought together by the following method
            CombineProfileInfo(id);
         
            loginAndBookList.booksAndDatabase = booksAndDatabase;

            loginAndBookList.loginModel.userid = id;
            loginAndBookList.loginModel.path = pathName;
            return View(loginAndBookList);
        }
        public ActionResult ProfileEdit1(int id)
        {
            DatabaseModel databaseModel = new DatabaseModel();
            User user = new User();
            String query = "SELECT *FROM USERS WHERE USERID = " + id;
            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);

            if (dataSet.Tables[0].Rows.Count == 1)
            {
                user.username = dataSet.Tables[0].Rows[0].ItemArray[1].ToString();
                user.useremail = dataSet.Tables[0].Rows[0].ItemArray[2].ToString();
                user.password = dataSet.Tables[0].Rows[0].ItemArray[3].ToString();
                user.bio = dataSet.Tables[0].Rows[0].ItemArray[5].ToString();
                // user.paths[0] = dataSet.Tables[0].Rows[0].ItemArray[4].ToString();
                return View("~/Views/Profile/ProfileEdit.cshtml", user);
            }

            return View("~/Views/Login/Login.cshtml");
        }
        [HttpPost]
        public ActionResult ProfileEdit1(User user)
        {
            string fileName = "";
            string filePath = "";
            var file = user.File[0];
            string query = "";
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    fileName = Path.GetFileName(file.FileName);
                    filePath = Path.Combine(Server.MapPath("~/images"), fileName);
                    file.SaveAs(filePath);
                    string toSave = "~/images/" + fileName;
                    if (!string.IsNullOrEmpty(user.password))
                    {
                        if (user.password == user.confirmPassword)
                        {
                            user.password = hashing.SHA512(user.password);
                            query = "UPDATE USERS SET PASSWORD = '" + user.password + "', photo = '" + toSave + "', bio = '" + user.bio + "' WHERE USERNAME = '" + Session["username"].ToString() + "'";
                        }
                        else
                        {
                            TempData["password"] = "<script> alert('password and confirm password do not match');</script>";
                            return View("~/Views/Profile/ProfileEdit.cshtml", user);
                        }
                    }
                    else
                    {
                        query = "UPDATE USERS SET photo = '" + toSave + "', bio = '" + user.bio + "' WHERE USERNAME = '" + Session["username"].ToString() + "'";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(user.password))
                    {
                        if (user.password == user.confirmPassword)
                        {
                            user.password = hashing.SHA512(user.password);
                            query = "UPDATE USERS SET PASSWORD = '" + user.password + "', bio = '" + user.bio + "' WHERE USERNAME = '" + Session["username"].ToString() + "'";

                        }
                        else
                        {
                            TempData["password"] = "<script> alert('password and confirm password do not match');</script>";
                            return View("~/Views/Profile/ProfileEdit.cshtml", user);
                        }
                    }
                    else
                    {
                        query = "UPDATE USERS SET bio = '" + user.bio + "' WHERE USERNAME = '" + Session["username"].ToString() + "'";
                    }
                }
            }
            catch
            {
                

            }

            DatabaseModel databaseModel = new DatabaseModel();
            databaseModel.update(query);

            DatabaseModel databaseModel1 = new DatabaseModel();
            User user1 = new User();
            query = "SELECT *FROM USERS WHERE USERID = " + (int)System.Web.HttpContext.Current.Session["Id"];
            DataSet dataSet = new DataSet();
            dataSet = databaseModel1.selectFunction(query);
            LoginModel loginModel = new LoginModel();
            loginModel.userid = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[0]);
            loginModel.username = dataSet.Tables[0].Rows[0].ItemArray[1].ToString();
            loginModel.path = dataSet.Tables[0].Rows[0].ItemArray[4].ToString();
            LoginAndBookList loginAndBookList = new LoginAndBookList();
            loginAndBookList.loginModel = loginModel;


            int id = (int)System.Web.HttpContext.Current.Session["Id"];


            return RedirectToAction("Profile", "Profile", new { id = id });


        }

        //This method is responsible for showing the books that have been published by the guest user
        private void GetPublishedList(int id)
        {
            String query = "SELECT *FROM BOOKS WHERE USERID = " + id;
            DataSet dataSet = new DataSet();
            dataSet = booksAndDatabase.databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Books books = new Books();
                    books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    books.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    books.bookCover = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                    booksAndDatabase.listOfBooks[1].Add(books);
                }
                loginAndBookList.loginModel.totalPublished = booksAndDatabase.listOfBooks[1].Count;
            }
        }


        //This method is responsible for showing the books that are in the library of the guest user
        //This method will be called every time someone requests to view a profile
        //But unless Session["Id"] matches the id that has been sent as the parameter of the Profile(int id)
        //method, that is, a host user is viewing the profile of a guest user, this list will not be shown
        //to the guest user, that is, this list is private to the owner of the account
        private void GetReadList(int id)
        {
            String query = "SELECT *FROM BOOKS WHERE BOOKID IN (SELECT BOOKID FROM READLOG WHERE USERID = " + id + ")";
            DataSet dataSet = new DataSet();
            dataSet = booksAndDatabase.databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    Books books = new Books();
                    books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    books.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    books.bookCover = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                    booksAndDatabase.listOfBooks[0].Add(books);
                }
            }
        }


        //This method is responsible for showing the profile photo, username and bio of the
        //guest user
        private void GetUserInformation(int id)
        {
            String query = "SELECT PHOTO, USERNAME, BIO FROM USERS WHERE USERID = " + id; ;
            DataSet dataSet = new DataSet();
            dataSet = booksAndDatabase.databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                pathName = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();

            }
            loginAndBookList.loginModel.username = dataSet.Tables[0].Rows[0].ItemArray[1].ToString();
            loginAndBookList.loginModel.bio = dataSet.Tables[0].Rows[0].ItemArray[2].ToString();
        }


        //This method retrieves the desired tags of the guest user. This will come in use while
        //creating recommendations for the user
        private void GetTags(int id)
        {
            string query = " SELECT *FROM USERTAG LEFT JOIN TAGS ON UserTag.TAGID = Tags.TagID WHERE USERID =" + id;
            DatabaseModel database = new DatabaseModel();
            DataSet dataSet = database.selectFunction(query);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Tags tag = new Tags();
                tag.tagId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                tag.tagName = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                booksAndDatabase.tagList.Add(tag);

            }
        }


        //This method is responsible for creating recommendation of books to the guest user
        //based on their desired tags
        //This method will be called every time someone requests to view a profile
        //But unless Session["Id"] matches the id that has been sent as the parameter of the Profile(int id)
        //method, that is, a host user is viewing the profile of a guest user, this list will not be shown
        //to the guest user, that is, this list is private to the owner of the account
        private void CreateRecommendation(int id)
        {
            DatabaseModel databaseModel = new DatabaseModel();
            int index = 0;
            foreach (Tags tag in booksAndDatabase.tagList)
            {
                booksAndDatabase.recommendation.Add(new List<Books>());
                List<Books> books = null;
                string query = " SELECT READLOG.BOOKID, BOOKS.BookName, BOOKS.BookCover, BOOKS.UserId, Books.Rating, BOOKS.BookCover, BOOKTAGS.TAGID FROM READLOG LEFT JOIN BOOKTAGS ON ReadLog.BookId = BookTags.BookId LEFT JOIN Books ON ReadLog.BookId = BOOKS.BookID WHERE BOOKTAGS.TAGID = " + tag.tagId + " GROUP BY READLOG.BookId, BookTags.TAGID, BOOKS.BookName, BOOKS.BookCover, BOOKS.Rating, BOOKS.UserId EXCEPT SELECT READLOG.BOOKID, BOOKS.BookName, BOOKS.BookCover, BOOKS.UserId, Books.Rating, BOOKS.BookCover, BOOKTAGS.TAGID FROM READLOG LEFT JOIN BOOKTAGS ON ReadLog.BookId = BookTags.BookId LEFT JOIN Books ON ReadLog.BookId = BOOKS.BookID WHERE BOOKTAGS.TAGID = " + tag.tagId + " AND ReadLog.UserId = " + System.Web.HttpContext.Current.Session["Id"] + " GROUP BY READLOG.BookId, BookTags.TAGID, BOOKS.BookName, BOOKS.BookCover, BOOKS.Rating, BOOKS.UserId";

                DataSet dataSet = databaseModel.selectFunction(query);
                books = new List<Books>();
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    Books book = new Books();
                    book.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                    book.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    book.bookCover = dataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                    book.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[3]);
                    // book.rating = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[4]);
                    books.Add(book);
                }

                booksAndDatabase.recommendation[index] = books;
                index++;
            }

        }

       
        private void GetTotalReaderCount(int id)
        {
             string query = "SELECT COUNT (USERID) FROM ReadLog WHERE BOOKID IN (SELECT BOOKID FROM BOOKS WHERE USERID = " + id + ")";
            DatabaseModel databaseModel = new DatabaseModel();
            
            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
            if(dataSet.Tables[0].Rows.Count>0)
            {
                loginAndBookList.loginModel.totalViews = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[0]);
            }
        }

        public ActionResult AddFollower(int id)
        {
            if(Session["Id"] == null)
            {
                TempData["notloggedin"] = "<script> alert('Please Login To Continue');</script>";
                return RedirectToAction("Profile", "Profile", new { @id = id});
            }
            int follower = (int)System.Web.HttpContext.Current.Session["Id"];
            string query = "INSERT INTO FOLLOW VALUES (" + follower + ", " + id + ")";
            DatabaseModel database = new DatabaseModel();
            database.insert(query);
            return RedirectToAction("Profile", "Profile", new { @id = id });
        }
        private void GetFollowerCount(int id)
        {
            string query = "SELECT COUNT (follower) FROM follow WHERE following = " + id;
            DatabaseModel databaseModel = new DatabaseModel();

            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                loginAndBookList.loginModel.followerNum = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[0]);
            }
        }

        private void GetFollowingState(int id)
        {
            int follower = (int)System.Web.HttpContext.Current.Session["Id"];
            string query = "SELECT *FROM FOLLOW WHERE follower = " + follower + " AND following ="+ id;
            DataSet dataset = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataset = databaseModel.selectFunction(query);
            if (dataset.Tables[0].Rows.Count > 0)
            {
                loginAndBookList.loginModel.isFollowing = 1;
            }
        }
        public ActionResult Unfollow(int id)
        {
            int follower = (int)System.Web.HttpContext.Current.Session["Id"];
            string query = "DELETE FROM FOLLOW WHERE follower = " + follower + " AND following =" + id;
            
            DatabaseModel databaseModel = new DatabaseModel();
            databaseModel.insert(query);

            return RedirectToAction("Profile", "Profile", new { @id = id });
            
        }
        //This method is responsible for getting the library, published list, profile picture
        //username, bio, preferred tags and reccommendations based on the tags of the guest user
        private void CombineProfileInfo(int id)
        {
            GetPublishedList(id);
            GetFollowerCount(id);
            GetTotalReaderCount(id);
            if (Session["Id"] != null)
            {
                GetReadList(id);
                GetUserInformation(id);
                GetTags(id);
                CreateRecommendation(id);
                GetFollowingState(id);
                
            }
            else
            {
                loginAndBookList.loginModel.isFollowing = 0;
            }
            
        }

       
    }
}