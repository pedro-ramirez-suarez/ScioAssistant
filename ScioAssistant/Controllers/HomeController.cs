using Irony.Parsing;
using Newtonsoft.Json;
using SearchLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScioAssistant.Controllers
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

        [HttpPost]
        public ActionResult Search(string query)
        {
            var lang = new CustomGrammar();
            ParseTree tree;
            Parser parser = new Parser(lang);
            tree = parser.Parse(query);

            var execute = lang.QueryStatement(tree.Root.ChildNodes[0]);
            var db = new CustomSearch.data.DataAccess("default");
            var result = db.ExecuteSearch(execute.Item1, execute.Item2, execute.Item3);

            return Content(JsonConvert.SerializeObject(result));
        }
    }
}