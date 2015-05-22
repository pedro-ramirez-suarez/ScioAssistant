using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLang
{
    [Language("Custom", "1", "Custom searches on natural language")]
    public class CustomGrammar : Grammar
    {
        public CustomGrammar()
            : base(false)
        {
            //Terminals
            var cuantos = ToTerm("cuantos");
            var cuantas = ToTerm("cuantas");
            var cual = ToTerm("cual");
            var que = ToTerm("que");
            var cuando = ToTerm("cuando");
            var tiene  = ToTerm("tiene");
            var esta = ToTerm("esta");
            var en = ToTerm("en");
            var antes = ToTerm("antes");
            var despues = ToTerm("despues");
            var de = ToTerm("de");
            var mas = ToTerm("mas");
            var menos = ToTerm("menos");
            var mayor = ToTerm("mayor");
            var menor = ToTerm("menor");

            var number = new NumberLiteral("number");
            var string_literal = new StringLiteral("string", "'", StringOptions.AllowsDoubledQuote);

            //non terminals
            var cuantosStatement = new NonTerminal("cuantosStatement");
            var queStatement = new NonTerminal("queStatement");
            var cuandoStatement = new NonTerminal("cuandoStatement");
            var cualMayorMenorStatement = new NonTerminal("queMayorMenorStatement");
            var cualMasMenosStatement = new NonTerminal("queMasMenosStatement");

            var tabla = new NonTerminal("tabla");
            var tabla2 = new NonTerminal("tabla2");
            
            var campo = new NonTerminal("campo");
            var id = new NonTerminal("id");
            var comando = new NonTerminal("comando");
            var whereId = new NonTerminal("whereId");
            var antesdespues = new NonTerminal("antesdespues");
            var cuantoscuantas = new NonTerminal("cuantoscuantas");
            var mayormenor = new NonTerminal("mayormenor");
            var masmenos = new NonTerminal("masmenos");

            var Id_simple = TerminalFactory.CreateSqlExtIdentifier(this, "id_simple"); //covers normal identifiers (abc) and quoted id's ([abc d], "abc d")
            id.Rule = Id_simple;
            tabla.Rule = id;
            tabla2.Rule = id;
            antesdespues.Rule = (antes | despues);
            cuantoscuantas.Rule = cuantos | cuantas;
            mayormenor.Rule = mayor | menor;
            masmenos.Rule = mas | menos;

            campo.Rule = id;
            whereId.Rule = string_literal | number;
            //Cuanto pregunta
            cuantosStatement.Rule = (cuantoscuantas + tabla + tiene + tabla2 + whereId) | (en + cuantoscuantas + tabla + esta + tabla2 + whereId);
            
            //que pregunta
            queStatement.Rule = (que + campo + tiene + tabla + whereId) | (que + tabla + campo + antesdespues + de +whereId );
            //que Mayor menor pregunta
           cualMayorMenorStatement.Rule = cual + tabla + tiene + mayormenor + campo;            
            //que menos mas pregunta
           cualMasMenosStatement.Rule = cual + tabla +  tiene + masmenos + tabla2;

            //cuando preguta
            cuandoStatement.Rule = cuando + campo + tabla + whereId;

            comando.Rule = cuantosStatement | queStatement | cuandoStatement | cualMasMenosStatement | cualMayorMenorStatement;
            this.Root = comando;
        }



        public Tuple<string, string, Dictionary<string, object>> QueryStatement(ParseTreeNode node)
        {
            //process the  node
            if (node.Term != null && !string.IsNullOrWhiteSpace(node.Term.Name))
            {
                switch (node.Term.Name)
                {
                    case "queStatement":
                        return Que(node);
                    case "cuandoStatement":
                        return Cuando(node);
                    case "cuantosStatement":
                        return Cuantos(node);
                    case "queMayorMenorStatement":
                        return QueMayorMenor(node);
                    case "queMasMenosStatement":
                        return QueMasMenos(node);
                    default :
                        return new Tuple<string, string, Dictionary<string, object>>(string.Empty, string.Empty, new Dictionary<string, object>());
                }
            }
            return new Tuple<string, string, Dictionary<string, object>>(string.Empty, string.Empty, new Dictionary<string, object>());
        }
        private Tuple<string, string, Dictionary<string, object>> Que(ParseTreeNode node)
        {
            //Que first we need to determine what kind of "que" question is this
            if (node.ChildNodes.Count == 5)
            {
                var campo = GetValueForTerm("campo", node);
                var tabla = GetValueForTerm("tabla", node);
                var where = GetValueForTerm("whereId", node);
                return new Tuple<string, string, Dictionary<string, object>>
                (string.Format("SELECT {0} FROM {1} WHERE [default] = @param1", campo, tabla),
                tabla,
                new Dictionary<string, object> { { "param1", ConvertParamToProperType(where) } });

            }
            else if (node.ChildNodes.Count == 6)
            {
                var campo = GetValueForTerm("campo", node);
                var tabla = GetValueForTerm("tabla", node);
                var where = GetValueForTerm("whereId", node);
                var cuando = GetValueForTerm("antesdespues", node);
                return new Tuple<string, string, Dictionary<string, object>>
                (string.Format("SELECT * FROM {0} WHERE {1} {2} @param1", tabla,campo, cuando == "antes" ? "<" : ">", where),
                tabla,
                new Dictionary<string, object> { { "param1", ConvertParamToProperType(where) } });
            }
            else
                return new Tuple<string, string, Dictionary<string, object>>(string.Empty, string.Empty, new Dictionary<string, object>());
        }

        private Tuple<string, string, Dictionary<string, object>> QueMayorMenor(ParseTreeNode node)
        {
            var campo = GetValueForTerm("campo", node);
            var tabla = GetValueForTerm("tabla", node);
            var cantidad = GetValueForTerm("mayormenor", node);
            return new Tuple<string, string, Dictionary<string, object>>
                (string.Format("SELECT top 1 [default],{0} FROM {1} order by {2} {3}",campo,tabla,campo,cantidad =="mayor" ? "DESC" : "ASC"),
                tabla ,
                new Dictionary<string, object> { });
            
        }


        private Tuple<string, string, Dictionary<string, object>> QueMasMenos(ParseTreeNode node)
        {
            var campo = GetValueForTerm("campo", node);
            var tabla = GetValueForTerm("tabla", node);
            var tabla2 = GetValueForTerm("tabla2", node);
            tabla2 = tabla2.EndsWith("es") ? tabla2.Remove(tabla2.Length - 2) : tabla2;
            tabla2 = tabla2.EndsWith("s") ? tabla2.Remove(tabla2.Length - 1) : tabla2;

            var cantidad = GetValueForTerm("masmenos", node);
            
            return new Tuple<string, string, Dictionary<string, object>>
                (string.Format("SELECT top 1 {0}Id as [default],count({1}Id) as Total from {2} GROUP BY {0}Id ORDER BY Total DESC",tabla,tabla2,tabla+ tabla2),
                tabla ,
                new Dictionary<string, object> { });
        }

        private Tuple<string,string,Dictionary<string,object>> Cuando(ParseTreeNode node)
        {
            var campo = GetValueForTerm("campo", node);
            var tabla = GetValueForTerm("tabla", node);
            var where = GetValueForTerm("whereId", node);

            return new Tuple<string, string, Dictionary<string, object>>
                (string.Format("SELECT {0} FROM {1} WHERE [default] = @param1", campo, tabla),
                tabla,
                new Dictionary<string, object> {{ "param1",ConvertParamToProperType(where)} });
        }


        private Tuple<string, string, Dictionary<string, object>> Cuantos(ParseTreeNode node)
        {
            //first we need to determine what kind of cuantos question is this
            if (node.ChildNodes.Count == 5)
            {
                var campo = GetValueForTerm("campo", node);
                var tabla = GetValueForTerm("tabla", node);
                tabla = tabla.EndsWith("es") ? tabla.Remove(tabla.Length - 2) : tabla;
                tabla = tabla.EndsWith("s") ? tabla.Remove(tabla.Length - 1) : tabla;
                var tabla2 = GetValueForTerm("tabla2", node);
                var where = GetValueForTerm("whereId", node);
                return new Tuple<string, string, Dictionary<string, object>>
                    (string.Format("SELECT count(ID) FROM {0} WHERE {1}= @param1", tabla2 + tabla, tabla2 + "Id"),
                    tabla2 + tabla,
                    new Dictionary<string, object> { { "param1", ConvertParamToProperType(where) } });
            }
            else if (node.ChildNodes.Count == 6)
            {
                var campo = GetValueForTerm("campo", node);
                var tabla = GetValueForTerm("tabla", node);
                tabla = tabla.EndsWith("es") ? tabla.Remove(tabla.Length - 2) : tabla;
                tabla = tabla.EndsWith("s") ? tabla.Remove(tabla.Length - 1) : tabla;
                var tabla2 = GetValueForTerm("tabla2", node);
                var where = GetValueForTerm("whereId", node);
                return new Tuple<string, string, Dictionary<string, object>>
                    (string.Format("SELECT count(ID) FROM {0} WHERE {1}= @param1", tabla + tabla2, tabla2 + "Id"),
                    tabla + tabla2,
                    new Dictionary<string, object> { { "param1", ConvertParamToProperType(where) } });
            }
            else
                return new Tuple<string, string, Dictionary<string, object>>(string.Empty, string.Empty, new Dictionary<string, object>());
        }

        private object ConvertParamToProperType(string val)
        {
            //try conversions
            int i;
            if (int.TryParse(val, out i))
                return i;
            decimal d;
            if (decimal.TryParse(val, out d))
                return d;
            DateTime dt;
            if (DateTime.TryParse(val, out dt))
                return dt;
            Guid g;
            if (Guid.TryParse(val, out g))
                return g;
            //is a string
            return val;
        }
        private string GetValueForTerm(string term, ParseTreeNode parent)
        {
            string val = string.Empty;
            if (parent.Term.Name == term)
                val = GetFinalToken(parent);
            else
            {
                foreach (var n in parent.ChildNodes)
                {
                    if (n.ChildNodes.Any())
                        val = GetValueForTerm(term, n);
                    if (!string.IsNullOrWhiteSpace(val))
                        break;
                }
            }
            return val;
        }

        private string GetFinalToken(ParseTreeNode node)
        {
            string val = string.Empty;
            if (node.ChildNodes.Any())
            {
                foreach(var n in node.ChildNodes)
                {
                    val = GetFinalToken(n);
                    if (!string.IsNullOrWhiteSpace(val))
                        break;
                }
            }
            else
                val = node.Token.Value.ToString();
            return val;
        }
    }
}
