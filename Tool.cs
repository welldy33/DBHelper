using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class Tool
    {
        public static DataRowCollection ToRows(DataSet ds, int dtIndex) => ds != null && ds.Tables.Count > dtIndex && ds.Tables[dtIndex].Rows.Count > 0 ? ds.Tables[dtIndex].Rows : (DataRowCollection)null;

        public static DataRowCollection ToRows(DataSet ds) => Tool.ToRows(ds, 0);
        public static List<Dictionary<string, object>> ToListDic(DataRow[] drs)
        {
            if (drs == null || drs.Length <= 0)
                return (List<Dictionary<string, object>>)null;
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
            foreach (DataRow dr in drs)
                dictionaryList.Add(Tool.ToDic(dr));
            return dictionaryList;
        }

        public static List<Dictionary<string, object>> ToListDic(DataRowCollection drs)
        {
            if (drs == null || drs.Count <= 0)
                return (List<Dictionary<string, object>>)null;
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
            foreach (DataRow dr in (InternalDataCollectionBase)drs)
                dictionaryList.Add(Tool.ToDic(dr));
            return dictionaryList;
        }

        public static List<Dictionary<string, object>> ToListDic(DataSet ds) => Tool.ToListDic(ds, 0);

        public static List<Dictionary<string, object>> ToListDic(DataSet ds, int dtIndex)
        {
            if (ds == null || ds.Tables.Count <= dtIndex || ds.Tables[dtIndex].Rows.Count <= 0)
                return (List<Dictionary<string, object>>)null;
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
            foreach (DataRow row in (InternalDataCollectionBase)ds.Tables[dtIndex].Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (DataColumn column in (InternalDataCollectionBase)ds.Tables[dtIndex].Columns)
                    dictionary.Add(column.ColumnName, row[column]);
                dictionaryList.Add(dictionary);
            }
            return dictionaryList;
        }


        public static Dictionary<string, object> ToDic(DataSet ds) => Tool.ToDic(ds, 0);

        public static Dictionary<string, object> ToDic(DataSet ds, int dtIndex) => ds != null && ds.Tables.Count > dtIndex && ds.Tables[dtIndex].Rows.Count > 0 ? Tool.ToDic(ds.Tables[dtIndex].Rows[0]) : (Dictionary<string, object>)null;

        public static Dictionary<string, object> ToDic(DataRow dr)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (DataColumn column in (InternalDataCollectionBase)dr.Table.Columns)
                dictionary.Add(column.ColumnName, dr[column]);
            return dictionary;
        }

        public static Dictionary<string, object> ToDic(string jsonStr) => jsonStr == null || jsonStr.Length == 0 ? new Dictionary<string, object>() : JavaScriptObjectDeserializer.DeserializeDic(jsonStr);
        public static Dictionary<string, object> ToDic(params object[] array)
        {
            if (array == null)
                return (Dictionary<string, object>)null;
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            int num;
            for (int index = 0; index < array.Length; index = num + 1)
                dictionary.Add(array[index].ToString(), array[num = index + 1]);
            return dictionary;
        }

        public static ArrayList ToList(string jsonStr) => jsonStr == null || jsonStr.Length == 0 ? (ArrayList)null : JavaScriptObjectDeserializer.DeserializeArrayList(jsonStr);

        public static DataRow ToRow(DataSet ds) => Tool.ToRow(ds, 0);

        public static DataRow ToRow(DataSet ds, int dtIndex) => ds != null && ds.Tables.Count > dtIndex && ds.Tables[dtIndex].Rows.Count > 0 ? ds.Tables[dtIndex].Rows[0] : (DataRow)null;
        


    }
}
