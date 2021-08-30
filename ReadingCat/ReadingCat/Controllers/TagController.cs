using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using ReadingCat.Models;

namespace ReadingCat.Controllers
{
    public class TagController : Controller
    {
 
        DataSet dataSet;
        Tags tags = new Tags();
        int returnedRows;
        // GET: Tag
        [HttpGet]
        public ActionResult ViewTags()
        {
            GetTags();
            returnedRows = dataSet.Tables[0].Rows.Count;
            CreateList();
            return View(tags);
        }

        [HttpPost]
        public ActionResult ViewTags(Tags tags)
        {
            if (tags.listOfTags.Count(m => m.isSelected) == 0)
            {
                return View(tags);
            }

            else
            {

                DatabaseModel databaseModel = new DatabaseModel();

                int currentUser = (int)System.Web.HttpContext.Current.Session["Id"];

                foreach (Tags tag in tags.listOfTags)
                {

                    if (tag.isSelected)
                    {
                        string query = "INSERT INTO USERTAG VALUES (" + System.Web.HttpContext.Current.Session["Id"] + "," + tag.tagId + ")";
                        databaseModel.insert(query);
                    }
                }

                return RedirectToAction("Profile", "Profile", new { id = currentUser });
            }
        }
        private void GetTags()
        {
            string query = "SELECT *FROM TAGS";
            DatabaseModel databaseModel = new DatabaseModel();
            dataSet = new DataSet();
            dataSet = databaseModel.selectFunction(query);
        }

        private void CreateList()
        {
            tags.listOfTags = new List<Tags>();
            for (int i = 0; i < returnedRows; i++)
            {
                Tags tag = new Tags();
                tag.tagId = Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[0]);
                tag.tagName = dataSet.Tables[0].Rows[i].ItemArray[1].ToString();
                tag.isSelected = false;
                tags.listOfTags.Add(tag);
            }
        }

        private void AddTags()
        {
            DatabaseModel databaseModel = new DatabaseModel();
            for (int i = 0; i < tags.listOfTags.Count; i++)
            {
                string query = "INSERT INTO USERTAG VALUES (" + System.Web.HttpContext.Current.Session["Id"] + "," + tags.listOfTags[i].tagId + ")";
                databaseModel.insert(query);
            }

        }
    }
}