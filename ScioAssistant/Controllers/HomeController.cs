using Irony.Parsing;
using Newtonsoft.Json;
using DeepThought.SmartAgent;
using SearchLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeepThought.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Database()
        {
            return View();
        }

        public ActionResult Preguntas()
        {
            return View();
        }

        public ActionResult Source()
        {
            return View();
        }


        /// <summary>
        /// this is used by windows phone to get results
        /// </summary>
        public ActionResult SearchForPhone(string query)
        {
            return Content(LocalSearch(query));   
        }


        /// <summary>
        /// this is used by the web application
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search(string query)
        {
            return Content(LocalSearch(query));   
        }

        public string LocalSearch(string query)
        {
            var deepThought = new DeepThoughtMachine();
            return JsonConvert.SerializeObject(deepThought.Search(query));
        }


        
    }
}