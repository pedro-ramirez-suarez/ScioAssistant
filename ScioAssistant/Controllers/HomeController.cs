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

        public ActionResult Database()
        {
            return View();
        }

        public ActionResult Preguntas()
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
            if (tree.Root == null || tree.Root.ChildNodes[0] == null)
            {
                //add single quotes in last word and try again
                var words = query.Split(new char[] {' '});
                words[words.Length - 1] = "'" + words[words.Length - 1] + "'";
                query = string.Join(" ", words);
                //if is still null, send a custom message
                tree = parser.Parse(query);
                if (tree.Root == null || tree.Root.ChildNodes[0] == null)
                {
                    return Content(JsonConvert.SerializeObject(new List<object> () {new { error = "Lo siento, no puedo entenderte, intenta de nuevo" }}));
                }
            }
            try
            {
                var execute = lang.QueryStatement(tree.Root.ChildNodes[0]);
                var db = new CustomSearch.data.DataAccess("default");
                var result = db.ExecuteSearch(execute.Item1, execute.Item2, execute.Item3);
                //Check if result returned something, if not, send custom msg
                return Content(JsonConvert.SerializeObject(result));
            }
            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(new List<object>() { new { error = "Lo siento, no puedo entenderte, intenta de nuevo" } }));
            }
            
            
        }
    }
}