using System;
using System.Data;
using System.Web.Mvc;
using ReadingCat.Models;


namespace ReadingCat.Controllers
{
    public class ReadController : Controller
    {
        // GET: Read
        DatabaseModel databaseModel = new DatabaseModel();
        DataSet dataSet = new DataSet();
        Books books = new Books();
        public ActionResult ReadBook(int id)
        {
            if (Session["Id"] == null)
            {
                TempData["notloggedin"] = "<script> alert('Please Login To Continue');</script>";
                return RedirectToAction("Login", "Login");
            }
            GetChapter(id);
            GetAllChapters(books.bookId);
            GetBookName(books.bookId);
            return View(books);
        }

        private void GetChapter(int id)
        {
            string query = "SELECT *FROM  BookChapters WHERE CHAPTERID = " + id;
            dataSet = databaseModel.selectFunction(query);
            books.currentChapter.chapterId = id;
            books.currentChapter.chapterName = dataSet.Tables[0].Rows[0].ItemArray[2].ToString();
            books.currentChapter.chatpterText = dataSet.Tables[0].Rows[0].ItemArray[3].ToString();
            books.currentChapter.approved = Convert.ToInt32( dataSet.Tables[0].Rows[0].ItemArray[4]);
            books.bookId = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[1]);
        }

        private void GetAllChapters(int id)
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
                books.chapters.Add(chapters);
            }
        }

        private void GetBookName(int id)
        {
            string query = "SELECT BOOKNAME FROM BOOKS WHERE BOOKID = " + id;
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            books.bookName = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();
        }
    }
}