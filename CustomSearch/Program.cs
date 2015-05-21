using Irony.Parsing;
using Newtonsoft.Json;
using SearchLang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSearch
{
    class Program
    {
        static CustomGrammar lang = new CustomGrammar();
        static Parser parser = new Parser(lang);
        //static ParseTree tree;

        static void Main(string[] args)
        {
            var lang = new CustomGrammar();
            Parser parser = new Parser(lang);
            ParseTree tree ;
            Console.WriteLine("Introduce la consulta");
            tree = parser.Parse(Console.ReadLine());
            PrintNode(tree.Root);
            Console.ReadKey();
        }

        static void PrintNode(ParseTreeNode node)
        {
            //Console.WriteLine(lang.QueryStatement(node.ChildNodes[0]));
            var execute = lang.QueryStatement(node.ChildNodes[0]);
            var db = new CustomSearch.data.DataAccess("default");
            var result = db.ExecuteSearch(execute.Item1, execute.Item2, execute.Item3);
            foreach (var i in result)
            {
                Console.WriteLine(JsonConvert.SerializeObject(i));
            }
                
           // db.ExecuteSearch()


            //Console.WriteLine("Token {0}, Tag {1}",node.Token,node.Tag);
            //if (node.ChildNodes.Any())
            //{
            //    foreach (var n in node.ChildNodes)
            //    {
            //        PrintNode(n);
            //    }
            //}
        }

        
    }
}
