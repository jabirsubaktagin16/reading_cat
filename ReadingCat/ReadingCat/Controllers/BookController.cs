using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ReadingCat.Models;
namespace ReadingCat.Controllers
{
    //This Controller is responsible for showing the details of a book
    //Which includes its book cover, author and genre name, number of times it has been read,
    //rating and chapters and the comment section
    //A user will also be able to place a comment on a book or give a rating through this controller
    public class BookController : Controller
    {
        DatabaseModel databaseModel;
        Books books;
        // GET: Book
        //This method is responsible for showing the book details
        public ActionResult BookDetails(int id)
        {
            databaseModel = new DatabaseModel();
            //Complete book details will be achieved through multiple database queries
            //This method will combine the results of those different queries
            combineBookDetails(id);
            return View(books);
        }
        
        //This method is reponsible for adding a new comment in a book
        [HttpPost]
        public ActionResult BookDetails(Books passedBook)
        {
            if (Session["Id"] == null)
            {
                TempData["notloggedin"] = "<script> alert('Please Login To Continue');</script>";
                return RedirectToAction("BookDetails", "Book", new { @id = passedBook.bookId });
            }
            string getComment = passedBook.currentComment.comment;
            int commenter = (int)System.Web.HttpContext.Current.Session["Id"];
            int bookCommented = (int)System.Web.HttpContext.Current.Session["CurrentBookId"];
            InsertComment(getComment, commenter, bookCommented);
            databaseModel = new DatabaseModel();
            combineBookDetails(bookCommented);
            return View(books);
        }

        //This method is responsible for getting the name of the book, its authoId, cover picture
        //summary and if the book is in the library of the user from the database
        private void GetBookDetails(int id)
        {
            string query = "SELECT *FROM BOOKS WHERE BOOKID = " + id;
            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                books.bookName = dataSet.Tables[0].Rows[0].ItemArray[1].ToString();
                books.userId = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[2]);
               // books.rating = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[3]);
                books.bookCover = dataSet.Tables[0].Rows[0].ItemArray[4].ToString();
                books.summary = dataSet.Tables[0].Rows[0].ItemArray[5].ToString();
            }

            //getting information about weather the book is in the user's library or not
            if (Session["Id"] != null)
            {
                GetLibraryState(id);
            }
        }


        //This method is responsible for getting the chapters of a book
        //If the user is the writer of the book or one of the admins
        //Then he will be able to view the unapproved chapters as well
        //Otherwise, the user will just be able to view the approved chapters
        private void GetBookChapters(int id)
        {
            string query = "SELECT *FROM BookChapters WHERE BOOKID = " + id;
            DataSet dataSet = new DataSet();
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Chapters chapters = new Chapters();
                chapters.chapterId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                chapters.chapterName = dataSet.Tables[0].Rows[i].ItemArray[2].ToString();
                chapters.chatpterText = dataSet.Tables[0].Rows[i].ItemArray[3].ToString();
                chapters.approved = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[4]);
                books.chapters.Add(chapters);
            }
        }

        //This method is responsible for adding the book to the user's library
        public ActionResult AddBook(int id)
        {
            if(Session["Id"] == null)
            {
                TempData["notloggedin"] = "<script> alert('Please Login To Continue');</script>";
                return RedirectToAction("BookDetails", "Book", new { @id = id });
            }
            int userId = (int)System.Web.HttpContext.Current.Session["Id"];
            string query = "INSERT INTO READLOG VALUES (" + userId + ", " + id + ")";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            databaseModel.insert(query);
            return RedirectToAction("BookDetails", "Book", new { @id = id });
        }

        //This method is responsible for getting information on weather the book is
        //in the library of the user or not
        private void GetLibraryState(int id)
        {
            string query = "SELECT *FROM READLOG WHERE USERID = " + (int)System.Web.HttpContext.Current.Session["Id"] + " AND BOOKID = " + id;
            DataSet dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
            //If there is an entry in the ReadLog in respect to the userid and the bookid
            //that means that the book exists in his library
            //If not, then the user does not have this book in his library
            //ReadLog table consists information about which user has read which books
            if (dataSet.Tables[0].Rows.Count == 1)
            {
                books.inLibrary = 1;
            }

            else
            {
                books.inLibrary = 0;
            }
        }


        //This method is responsible for getting the name of the author of the book
        //Since the "Book" table contains only the author's id as userId in the database
        //We have to conduct a seperate query for getting the author's name
        private void GetAuthorName(int id)
        {
            string query = "SELECT USERNAME FROM USERS WHERE USERID = " + id;
            DataSet dataSet = new DataSet();
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count >= 1)
            {
                books.author = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();
            }
        }


        //This method is responsible for retrieving the comments on the book
        private void GetComments(int id)
        {
            string query = "SELECT *FROM COMMENTS WHERE BOOKID = " + id;
            DataSet dataSet = new DataSet();
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                Comment comment = new Comment();
                comment.userId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[1]);
                comment.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[2]);
                comment.comment = dataSet.Tables[0].Rows[i].ItemArray[3].ToString();
                books.comments.Add(comment);
            }
        }


        //This method is responsible for getting the name of the person who put a 
        //new comment on the book
        //The cshtml returns only the userid of the commenter and his comment
        //So we have to conduct a seperate method to retrieve the username from the usrid from
        //our database
        private void GetCommenterName()
        {
            int i = 0;
            foreach (Comment comment in books.comments)
            {
                int userid = comment.userId;
                string query = "SELECT USERNAME FROM USERS WHERE USERID = " + userid;
                DataSet dataSet = new DataSet();
                dataSet = databaseModel.selectFunction(query);
                string username = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();
                books.comments[i].username = username;
                i += 1;
            }
        }


        //This method is responsible for inserting the new comment into the database
        private void InsertComment(string comment, int commenter, int bookCommented)
        {
            
            string connectionString = @"Data Source = DESKTOP-BKFDVUR\SQLEXPRESS; Initial Catalog = ReadingCat; Integrated Security = True";
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                string query = "INSERT INTO COMMENTS VALUES (@commenter, @bookCommented, @comment)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@commenter", commenter);
                sqlCommand.Parameters.AddWithValue("@bookCommented", bookCommented);
                sqlCommand.Parameters.AddWithValue("@comment", comment);

                sqlCommand.ExecuteNonQuery();
            }
        }


        //This method is responsible for getting the tag/ genre of the book
        //Since the "Book" table contains only the tag's id as tagId in the database
        //We have to conduct a seperate query for getting the tag's name
        private void GetBookTag(int id)
        {
            string query = "SELECT TAGNAME FROM TAGS WHERE TAGID = ( SELECT TAGID FROM BOOKTAGS WHERE BOOKID = " + id + ")";
            DataSet dataSet = new DataSet();
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            books.tag = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();

        }

        //This method is responsible for retieving the number of times the book has been read 
        //from the databse
        //Aggregate functions are being used to retrieve this information
        private void GetReadCount(int id)
        {
            string query = " SELECT COUNT(BOOKID), BOOKID FROM READLOG WHERE BOOKID = " + id + " GROUP BY BOOKID";
            DataSet dataSet = new DataSet();
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                books.readCount = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[0]);
            }

            else
            {
                books.readCount = 0;
            }
        }


        public void Rating(int id, int star)
        {
           
                   // string query = "insert into reads(rating) Values(" + star + ") where MangaID=" + id + " AND userid=" + currentUser.ID + "";
                    //string query = "update reads set rating=" + star + " where mangaid=" + id + " and userid=" + currentUser.ID;
              

          
        }

        //This method is responsible for combining all the details of the book
        //that are obtained from different database queries
        private void combineBookDetails(int id)
        {
            books = new Books();
            books.chapters = new List<Chapters>();
            GetBookDetails(id);
            GetBookChapters(id);
            GetAuthorName(books.userId);
            GetComments(id);
            GetCommenterName();
            GetBookTag(id);
            GetReadCount(id);
            books.bookId = id;
            Session["CurrentBookId"] = id;
        }
    }
}