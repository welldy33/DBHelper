using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DBHelper
{
    public class Instance
    {
        static SqlConnection connection() {
            SqlConnectionStringBuilder sConnB = new SqlConnectionStringBuilder()
            {
                DataSource = "192.168.3.182,1433",
                InitialCatalog = "Develop",
                UserID = "sa",
                Password = "Digiman182"
            };
            return new SqlConnection(sConnB.ConnectionString);
        }
        static public DataTable Query() {
            string strQuery = "select top 100 * from mos_patient";
            SqlConnection cn = connection();
            cn.Open();
            SqlCommand command = new SqlCommand(strQuery,cn);
            var a= command.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(a);
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["a"] = "b";
            return dataTable;
        }
    }
}
