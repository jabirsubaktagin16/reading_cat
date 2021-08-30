using ReadingCat.Models;
using ReadingCat.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

//This Controller can only be used by the admins of ReadingCat
//This Controller is responsible for viewing approving the chapters that the writers publish in the website
namespace ReadingCat.Controllers
{
    public class AdminController : Controller
    {
        //This object contains a list of unique books that contain unapproved chapters
        UnapprovedChapters aproveChapters = new UnapprovedChapters();
        //DatabaseModel is used to access database functions such as insert, update, select
        DatabaseModel databaseModel;
        
        // GET: Approve
        //This method is responsible for viewing the unique books that have 
        //unapproved chapters
        
        public ActionResult ViewUnapprovedBooks()
        {
            //By setting review to 1, we are letting the admin enter the review mode
            //He can now view and approve chapters
            Session["review"] = 1;    
            GetUniqueBookList();
            return View(aproveChapters);
        }
       

        //This method is responsible for approving and publishing an unapproved chapter
        public ActionResult Publish(int id)
        {
            //Setting the Approved attribute to 1 in the database
            //This will make the newly published chapter visible to the users
            String query = "UPDATE BOOKCHAPTERS SET APPROVED = 1 WHERE CHAPTERID = " + id;
            databaseModel = new DatabaseModel();
            databaseModel.update(query);
            return RedirectToAction("ReadBook", "Read", new { @id = id });
        }

        public ActionResult Disapprove(int id)
        {
            string query = "DELETE FROM BOOKCHAPTERS WHERE CHAPTERID = " + id;
            DatabaseModel databaseModel = new DatabaseModel();
            databaseModel.insert(query);
            return RedirectToAction("ReadBook", "Read", new { @id = id });

        }

        [HttpGet]
        public ActionResult SearchUser()
        {
            return View();

        }

        [HttpPost]
        public ActionResult UserList(User id)
        {

            List<User> user = new List<User>();
            GetUserList(user, id.username);
            return View(user);

        }
       
        public ActionResult UserDetails(string id)
        {

            User user = new User();
            GetUserDetails(user, id);
            return View(user);

        }

        [HttpGet]
        public ActionResult AddNewGenre()
        {       
            return View();
        }
        [HttpPost]
        public ActionResult AddNewGenre(Tags id)
        {
            List<string> listOfTags = new List<string>();
            string query = "SELECT *FROM TAGS";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if(dataSet.Tables[0].Rows.Count>0)
            {
                for(int i=0;i<dataSet.Tables[0].Rows.Count;i++)
                {
                    listOfTags.Add(dataSet.Tables[0].Rows[i].ItemArray[0].ToString());
                }
            }

            if(!listOfTags.Contains(id.tagName))
            {
                query = "INSERT INTO TAGS VALUES ('" + id.tagName + "')";
                databaseModel.insert(query);
            }
            TempData["tag"] = "<script> alert('Added the new tag');</script>";
            Tags tags = new Tags();
            return View(tags);

        }

        [HttpPost]
        public ActionResult AddAdmin(User user)
        {
            String query = "UPDATE USERS SET ISADMIN = 1 WHERE USERNAME = '" + user.username + "'";
            DatabaseModel databaseModel = new DatabaseModel();
            databaseModel.update(query);
            return RedirectToAction("UserDetails", new { @id = user.username });
        }

        
        private void GetUserDetails(User user, string userName)
        {
            string query = "SELECT USERNAME, PHOTO, ISADMIN, USERID, USEREMAIL FROM USERS WHERE USERNAME = '" + userName+"'";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if(dataSet.Tables[0].Rows.Count > 0)
            {
                
                    
                    user.username = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();
                    user.paths = dataSet.Tables[0].Rows[0].ItemArray[1].ToString();
                    if (String.IsNullOrEmpty(user.paths))
                    {
                        user.paths = "~/images/profile.png";
                    }
                    user.isAdmin = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[2]);
                    user.userid = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[3]);
                    user.useremail = dataSet.Tables[0].Rows[0].ItemArray[4].ToString();

            }
        }

        private void GetUserList(List<User> user, string userName)
        {
            string query = "SELECT USERNAME, PHOTO, ISADMIN FROM USERS WHERE USERNAME LIKE '%" + userName + "%'";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    User userToAdd = new User();
                    userToAdd.username = dataSet.Tables[0].Rows[i].ItemArray[0].ToString();
                    userToAdd.paths = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                    if (String.IsNullOrEmpty(userToAdd.paths))
                    {
                        userToAdd.paths = "~/images/profile.png";
                    }
                    userToAdd.isAdmin = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    user.Add(userToAdd);
                }
            }
        }
        //This method is responsible for retrieving the unique booklist from the database

        private void GetUniqueBookList()
        {
            string query = "SELECT DISTINCT BOOKS.BOOKID, BOOKNAME, USERID, RATING, BOOKCOVER, SUMMARY, TAGS.TagName FROM BOOKS JOIN BookChapters ON BOOKS.BookID = BookChapters.BookId LEFT JOIN BookTags ON BOOKS.BookID = BookTags.BookId LEFT JOIN Tags ON BOOKTAGS.TAGID = Tags.TagID WHERE APPROVED = 0";
            DatabaseModel databaseModel = new DatabaseModel();
            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Books books = new Books();
                books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                books.bookName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                books.bookCover = dataSet.Tables[0].Rows[i].ItemArray[4].ToString();
                books.summary = dataSet.Tables[0].Rows[i].ItemArray[5].ToString();
                books.tag = dataSet.Tables[0].Rows[i].ItemArray[6].ToString();
               // books.reviewing = 1;
                aproveChapters.unapprovedListOfBooks.Add(books);
            }
        }
        
    }
}