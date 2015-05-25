using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomSearch.data
{
    public class DataAccess
    {

        public Dictionary<string,string> Tables { get; set; }

        private string ConnectionStringName { get; set; }
        public DataAccess(string connectionStringName) 
        {
            this.ConnectionStringName = connectionStringName;
            Tables = new Dictionary<string, string>();
            Tables.Add("contrato","Numero" );
            Tables.Add("embarcacion", "nombre" );
            Tables.Add("contratoembarcacion", "Id" );
        }

        public List<dynamic> ExecuteSearch(string command, string tableName, Dictionary<string,object> pars)
        {
            var result = new List<dynamic>();
            //replace the tag with the default field for each table
            command = command.Replace("[default]", Tables[tableName]);
            try
            {
                //prepare connection and open it
                var conn = new SqlConnection(ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString);
                var cmd = new SqlCommand(command, conn);
                conn.Open();
                //add the parameters
                foreach (var e in pars)
                    cmd.Parameters.AddWithValue(e.Key, e.Value);
                //execute the query
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    var fields = new List<string>();
                    while (reader.Read())
                    {
                        //get all the fields
                        if (fields.Count == 0)
                        {
                            for (var x = 0; x < reader.FieldCount; x++)
                                fields.Add(reader.GetName(x));
                        }
                        var newItem = new ExpandoObject();
                        result.Add(newItem);
                        var item = newItem as IDictionary<string, object>;
                        foreach (var f in fields)
                            item.Add(f, reader[f]);
                    }
                }
                conn.Close();
            }

            catch (Exception e)
            {
                //add a custom message
                dynamic error = new ExpandoObject();
                error.error = "Lo siento, no puedo entenderte, intenta de nuevo" ;
                result.Add(error);
            }
            finally 
            {
            }
            return result;
        }
    }
}
