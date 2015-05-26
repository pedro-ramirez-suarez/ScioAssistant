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

        public ActionResult Source()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(string query)
        {
            //replace accents and trim the query
            query = query.Replace("á", "a").Trim();
            query = query.Replace("í", "i");
            query = query.Replace("ó", "o");
            query = query.Replace("ú", "u");
            query = query.Replace("é", "e");

            var lang = new CustomGrammar();
            ParseTree tree;
            Parser parser = new Parser(lang);
            tree = parser.Parse(query);
            //if the statement is not recognized, try to add single quotes in the last word and try again
            if (tree.Root == null || tree.Root.ChildNodes[0] == null)
            {
                var words = query.Split(new char[] {' '});
                words[words.Length - 1] = "'" + words[words.Length - 1] + "'";
                query = string.Join(" ", words);
                //if is still null, return an error
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
                //return the results as Json.Net string, we use Json.Net because expando objects are not serialized correcty by default serializator
                return Content(JsonConvert.SerializeObject(result));
            }
            catch (Exception e)
            {
                return Content(JsonConvert.SerializeObject(new List<object>() { new { error = "Lo siento, no puedo entenderte, intenta de nuevo" } }));
            }
            
            
        }
    }
}