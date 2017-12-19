using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
namespace DxAddIn
{
    class GetData
    {
        public static DataTable CreateTable()
        {
            try
            {
                var dt = new DataTable();
                var sheet = Globals.ThisAddIn.Application.ActiveSheet as Excel.Worksheet;
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
