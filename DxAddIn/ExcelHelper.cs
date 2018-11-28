using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Excel = Microsoft.Office.Interop.Excel;
namespace DxAddIn
{
    public class ExcelHelper
    {
        public static Excel.Application App => Globals.ThisAddIn.Application;
        public static Excel.Workbook Workbook => App.ActiveWorkbook;
        public static Excel.Worksheet Worksheet => Workbook.ActiveSheet;
        public static Excel.Range SelectRange => Worksheet.Application.Selection as Excel.Range;
        public static Excel.Sheets Sheets => Workbook.Worksheets;
        public static Excel.Workbooks Books => App.Workbooks;


        public static object[,] ListToObjArr<T>(IList<T> o)
        {
            Type type = typeof(T);
            var props = type.GetProperties();
            var rows = o.Count;
            var cols = props.Count();
            var result = new object[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = props[j].GetValue(o[i], null);
                }
            }
            return result;
        }
        public static List<T> ObjArrToListV<T>(object[,] o) where T : class
        {
            var rows = o.GetLength(0);
            var cols = o.GetLength(1);
            Type type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = new List<T>();
            for (int i = 1; i <= cols; i++)
            {
                var obj = Activator.CreateInstance(type);
                for (int j = 1; j <= rows; j++)
                {
                    //(T)Convert.ChangeType(o[i,j], typeof(T));
                    props[j - 1].SetValue(obj, o[j, i], null);
                }
                result.Add(obj as T);
            }
            return result;
        }
        public static List<T> ObjArrToList<T>(object[,] o) where T : class
        {
            var rows = o.GetLength(0);
            var cols = o.GetLength(1);
            Type type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = new List<T>();

            for (int i = 1; i <= rows; i++)
            {
                var obj = Activator.CreateInstance(type);
                for (int j = 1; j <= cols; j++)
                {
                    //(T)Convert.ChangeType(o[i,j], typeof(T));
                    props[j - 1].SetValue(obj, o[i, j], null);
                }
                result.Add(obj as T);
            }
            return result;
        }
        public static DataTable GetDataTable()
        {
            try
            {
                var dt = new DataTable();
                var sheet = Worksheet;
                var range = sheet.Application.Selection as Excel.Range;
                object[,] o = range.SpecialCells(Excel.XlCellType.xlCellTypeVisible).get_Value();
                var table = ChangeList(o);
                for (int i = 0; i < table[0].Length; i++)
                {
                    try
                    {
                        dt.Columns.Add(table[0][i], typeof(string));
                    }
                    catch (DuplicateNameException)
                    {
                        System.Windows.Forms.MessageBox.Show("列名有重复");
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                        break;
                    }
                }
                for (int j = 1; j < table.Count; j++)
                {
                    DataRow dr = dt.NewRow();
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        dr[k] = table[j][k];
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch
            {
                return null;
            }

        }
        public static List<string[]> ChangeList(object[,] o)
        {
            List<string[]> list = new List<string[]>();
            for (int i = 1; i <= o.Length / o.GetLength(1); i++)
            {
                string[] s = new string[o.GetLength(1)];
                for (int j = 0; j < s.Length; j++)
                {
                    s[j] = o[i, j + 1] == null ? "" : o[i, j + 1].ToString();
                }
                list.Add(s);
            }
            return list;
        }
    }
}
