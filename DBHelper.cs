using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace Helper
{
    public class DBHelper
    {
        static Dictionary<string, object> DBCatalog = Tool.ToDic(
            "DEV","Develop"
            ,"MAIN","PWI_MAIN"
            );
        static Dictionary<string, object> MethodCatalog = Tool.ToDic(
            "insert",true,
            "update",true,
            "delete",true
            );
        static SqlConnection connectDB(string db)
        {
            if (!DBCatalog.ContainsKey(db))
                throw new Exception("Cannot Found DB");
            SqlConnectionStringBuilder sConnB = new SqlConnectionStringBuilder() { DataSource = "192.168.3.182,1433", InitialCatalog = DBCatalog[db].ToString(), UserID = "sa", Password = "Digiman182" };
            return new SqlConnection(sConnB.ConnectionString);
        }
        static public string GetQueryStr(string path, IHostingEnvironment _hostingEnv) {
            string[] arr = path.Split('.');
            string pathFile = "";
            arr = arr.Take(arr.Count() - 1).ToArray();
            string fileNamestr = arr[arr.Length - 1];
            arr = arr.Take(arr.Count() - 1).ToArray();
            foreach (var str in arr)
            {
                pathFile += ("\\" + str);
            }
            var a = _hostingEnv.WebRootPath;
            var fileName = Path.GetFileName(fileNamestr+".sql");
            var filePath = Path.Combine(_hostingEnv.WebRootPath, "_service\\_sqlHelper"+pathFile+"\\", fileName);
            string sqlStr = System.IO.File.ReadAllText(filePath);
            return sqlStr;
        }
        static public DataSet Query(string path, Dictionary<string, object> arg, IHostingEnvironment _hostingEnv)
        {
            string[] arr = path.Split('.');
            string db = arr[arr.Length - 1];
            return Query(GetQueryStr(path, _hostingEnv), db, arg);
        }
        static public DataSet Query(string cmdName,string db,Dictionary<string,object> arg)
        {
            DbTransaction transaction = (DbTransaction)null;
            DataSet dataSet = new DataSet();
            try
            {
                SqlConnection cn = connectDB(db);
                cn.Open();
                SqlCommand command = createArgument(new SqlCommand(cmdName.ToUpper(), cn), arg);
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(dataSet);
                cn.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return dataSet;
        }
        static SqlCommand createArgument(SqlCommand cmd,Dictionary<string,object>arg) {
            SqlParameter param = new SqlParameter();
            foreach (string key in arg.Keys) {
                param.ParameterName = "@" + key.ToUpper();
                param.Value = arg[key];
                cmd.Parameters.Add(param);
            }
            return cmd;
        }
        static void mappingExecute(string cmdString,Dictionary<string,object>arg) {
            string method = cmdString.Split('#')[0];
            if (!MethodCatalog.ContainsKey(method.ToLower()))
                throw new Exception("Cannot Found Command");
            var arr = cmdString.Split('#')[1].Split('@');
            string table = arr[0];
            string db = arr[1];
            if (method.ToLower() == "insert") {
                Insert(table, db, arg);
            }
        }
        static public void Execute(string cmdString,Dictionary<string,object>arg){
            mappingExecute(cmdString, arg);
        }
        static void Insert(string table,string db,Dictionary<string,object>arg) {
            ArrayList strCmd = new ArrayList();
            List<string> listKey = new List<string>();
            List<string> listval = new List<string>();
            foreach (string key in arg.Keys) {
                listKey.Add(key);
                listval.Add("'"+arg[key].ToString()+"'" );
            }
            string cmdKey = string.Join(",", listKey.ToArray());
            string cmdVal = string.Join(",", listval.ToArray());
            string cmd = "Insert into " + table + "(" + cmdKey + ") values (" + cmdVal + ")";
            SqlConnection cn = connectDB(db);
            cn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand(cmd, cn);
            command.ExecuteNonQuery();
            cn.Close();
        }
        void Update() { }
        void Delete() { }
    }

}
