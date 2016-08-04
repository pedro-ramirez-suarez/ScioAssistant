using Irony.Parsing;
using SearchLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepThought.SmartAgent
{
    public class DeepThoughtMachine
    {
        public IEnumerable<object> Search(string query)
        {
            query = query.Replace("á", "a").Trim();
            query = query.Replace("í", "i");
            query = query.Replace("ó", "o");
            query = query.Replace("ú", "u");
            query = query.Replace("é", "e");

            //remove any dot or comma 
            query = query.Replace(".", "");
            query = query.Replace(",", "");
            query = query.Replace("¿", "");
            query = query.Replace("?", "");
            query = query.Replace("!", "");
            query = query.Replace("¡", "");

            var lang = new NaturalLanguageGrammar();
            ParseTree tree;
            Parser parser = new Parser(lang);
            tree = parser.Parse(query);
            //if the statement is not recognized, try to add single quotes in the last word and try again
            if (tree.Root == null || tree.Root.ChildNodes[0] == null)
            {
                var words = query.Split(new char[] { ' ' });
                words[words.Length - 1] = "'" + words[words.Length - 1] + "'";
                query = string.Join(" ", words);
                //if is still null, return an error
                tree = parser.Parse(query);
                if (tree.Root == null || tree.Root.ChildNodes[0] == null)
                {
                    return new List<object>() { new { error = "Lo siento, no puedo entenderte, pero la respuesta a la pregunta última sobe la vida, el universo y todo es 42" } };  
                }
            }
            try
            {
                var execute = lang.QueryStatement(tree.Root.ChildNodes[0]);
                var db = new CustomSearch.data.DataAccess("default");
                var result = db.ExecuteQuery(execute.Item1, execute.Item2, execute.Item3);
                //return the results as Json.Net string, we use Json.Net because expando objects are not serialized correcty by default serializator
                return result; // JsonConvert.SerializeObject(result);
            }
            catch 
            {
                return new List<object>() { new { error = "Lo siento, no puedo entenderte, pero la respuesta a la pregunta última sobe la vida, el universo y todo es 42" } };  
            }
        }
    }
}
