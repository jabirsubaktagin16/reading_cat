using System;
using System.Web.Mvc;
using System.Data;
using ReadingCat.Models;
using ReadingCat.ViewModel;
using System.Collections.Generic;

namespace ReadingCat.Controllers
{
    public class LoginController : Controller
    {
 
        private int userid;
        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            Session["Id"] = 0;
            return View(new DatabaseCombinedWithOtherModel());
        }
        [HttpPost]
        public ActionResult Login(DatabaseCombinedWithOtherModel model)
        {
            string realPassword = "";
            string paswordFromUser = "";

            List<string> listOfUserName = new List<string>();
            GetAllUserName(listOfUserName);
            Boolean validUser = CheckValidUser(model.LoginModel.username, listOfUserName);
            if(validUser == false)
            {
                TempData["valid"] = "<script> alert('username not recognized');</script>";
                return RedirectToAction("Login", "Login");
            }
            string query = "SELECT password, userid, photo, bio, isadmin FROM USERS WHERE username = '" + model.LoginModel.username + "'";
            string photo = "";
            DataSet dataSet;
            DatabaseModel databaseModel = model.DatabaseModel;
            databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            realPassword = dataSet.Tables[0].Rows[0].ItemArray[0].ToString();
            paswordFromUser = model.LoginModel.password;
            Hashing hashing = new Hashing();
            paswordFromUser = hashing.SHA512(paswordFromUser);
            if (realPassword == paswordFromUser)
            {
                userid = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[1]);
               // photo = dataSet.Tables[0].Rows[0].ItemArray[2].ToString();
               // bio = dataSet.Tables[0].Rows[0].ItemArray[3].ToString();
                int isAdmin = Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[4]);
                //model.LoginModel.userid = userid;
                //model.LoginModel.path = photo;
                //model.LoginModel.bio = bio;
                //model.LoginModel.isAdmin = isAdmin;
                //LoginAndBookList loginAndBookList = new LoginAndBookList();
                //loginAndBookList.loginModel = new LoginModel();
                //loginAndBookList.loginModel = model.LoginModel;
                //loginAndBookList.loginModel.userid = userid;
                //loginAndBookList.loginModel.username = model.LoginModel.username;
                //loginAndBookList.loginModel.bio = model.LoginModel.bio;
                Session["Id"] = userid;
                Session["username"] = model.LoginModel.username;
                Session["bio"] = model.LoginModel.bio;
                Session["Picture"] = photo;
                if (isAdmin == 1)
                {
                    Session["admin"] = 1;
                    Session["review"] = 0;
                }
                else
                {
                    Session["admin"] = 0;
                    Session["review"] = 0;
                }
                Boolean newUser = CheckTags();
                if (newUser)
                {
                    return RedirectToAction("ViewTags", "Tag");
                }
                else
                {
                    return RedirectToAction("Profile", "Profile", new { id = userid });
                }
            }
            else
                TempData["msg"] = "<script> alert('Wrong Password!');</script>";
                return View();
        }

        private Boolean CheckTags()
        {
            string query = "SELECT *FROM USERTAG WHERE USERID = " + System.Web.HttpContext.Current.Session["Id"];
            DatabaseModel database = new DatabaseModel();
            DataSet set = database.selectFunction(query);
            if (set.Tables[0].Rows.Count == 0)
                return true;
            else
                return false;
        }

        private void GetAllUserName(List<string> listOfUserName)
        {
            string query = "SELECT USERNAME FROM USERS";
            DataSet dataSet = new DataSet();
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = databaseModel.selectFunction(query);
            for(int i=0;i<dataSet.Tables[0].Rows.Count; i++)
            {
                listOfUserName.Add(dataSet.Tables[0].Rows[i].ItemArray[0].ToString());
            }
        }

        private Boolean CheckValidUser(string username,List<string> listOfUserName)
        {
            if(listOfUserName.Contains(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}