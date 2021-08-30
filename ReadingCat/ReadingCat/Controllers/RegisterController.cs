using System.Web.Mvc;
using ReadingCat.Models;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace ReadingCat.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        string connectionString = @"Data Source = DESKTOP-BKFDVUR\SQLEXPRESS; Initial Catalog = ReadingCat; Integrated Security = True";
        [HttpGet]
        public ActionResult Register1()
        {
            return View(new User());
        }

        [HttpPost]
        public ActionResult Register1(User user)
        {
            List<string> listOfUsername = new List<string>();
            List<string> listOfEmail = new List<string>();
            getAllUsername(listOfUsername);
            getAllEmail(listOfEmail);
            string tempUserName = user.username;
            string tempEmail = user.useremail;
            Hashing hashing = new Hashing();
            user.password = hashing.SHA512(user.password);
            if (!listOfUsername.Contains(tempUserName) && !listOfEmail.Contains(tempEmail))
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string query = "INSERT INTO USERS VALUES (@username, @useremail, @password, null, null, 0)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@username", user.username);
                    sqlCommand.Parameters.AddWithValue("@useremail", user.useremail);
                    sqlCommand.Parameters.AddWithValue("@password", user.password);
                    string conpass = user.confirmPassword;
                    sqlCommand.ExecuteNonQuery();
                }
                TempData["complete"] = "<script> alert('Registration Complete');</script>";
                return View("~/Views/Login/Login.cshtml");
            }
           
            else if(listOfUsername.Contains(tempUserName))
            {
                TempData["username"] = "<script> alert('Sorry this username already exists. Please try a new one');</script>";
                return View();
            }

            else if (listOfEmail.Contains(tempEmail))
            {
                TempData["email"] = "<script> alert('Sorry this email address already exists. Please try a new one');</script>";
                return View();
            }
            return View();
        }

        private void getAllUsername(List<string> listOfUsername)
        {
            string query = "SELECT USERNAME FROM USERS";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if(dataSet.Tables[0].Rows.Count>0)
            {
                for(int i = 0; i < dataSet.Tables[0].Rows.Count;i++)
                {
                    listOfUsername.Add(dataSet.Tables[0].Rows[i].ItemArray[0].ToString());
                }
            }

        }

        private void getAllEmail(List<string> listOfEmail)
        {
            string query = "SELECT USEREMAIL FROM USERS";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            if (dataSet.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    listOfEmail.Add(dataSet.Tables[0].Rows[i].ItemArray[0].ToString());
                }
            }

        }
    }
}