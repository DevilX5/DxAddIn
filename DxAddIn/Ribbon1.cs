using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using NPinyin;
using System.Data;
using System.Threading.Tasks;

namespace DxAddIn
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void button2_Click(object sender, RibbonControlEventArgs e)
        {
            var tp = new DxTempTableViewer();
            var p = Globals.ThisAddIn.CustomTaskPanes.Add(tp, "转换");
            tp.Dock = DockStyle.Fill;
            p.Width = 600;
            p.Visible = true;
            var t = Task.Run(() =>
            {
                var dt = GetData.CreateTable();
                dt.Columns.Add("全拼");
                dt.Columns.Add("首字母");
                if (dt != null)
                {
                    var i = 0;
                    var total = dt.Rows.Count;
                    foreach (DataRow dr in dt.Rows)
                    {
                        i++;
                        dr["全拼"] = Pinyin.GetPinyin(dr[0].ToString());
                        dr["首字母"] = Pinyin.GetInitials(dr[0].ToString());
                        tp.Invoke((MethodInvoker)delegate
                        {
                            tp.TsMsg.Text = $"正在计算第{i}行,共{total}行";
                        });
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("请选择一个以上的单元格");
                }
                return dt;
            });
            Task.Run(() =>
            {
                Task.WaitAll(t);
                tp.Invoke((MethodInvoker)delegate
                {
                    tp.Dt = t.Result;
                    tp.TsMsg.Text = $"数据获取完毕,共计{t.Result.Rows.Count}行";
                });
            });
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            var ws = CurrentApp.App.ActiveWorkbook.ActiveSheet as Excel.Worksheet;
            var range = ws.Application.Selection as Excel.Range;
            var zhlst = new List<string>();
            var enlst = new List<string>();
            var values = range.Cast<Excel.Range>().SelectMany()
            //var tp = new DxTempTableViewer();
            //var p = Globals.ThisAddIn.CustomTaskPanes.Add(tp, "转换");
            //tp.Dock = DockStyle.Fill;
            //p.Width = 600;
            //p.Visible = true;
            //tp.Dt = values;
        }
        
    }
    public class ZhEnField
    {
        public string ZhName { get; set; }
        public string EnName { get; set; }
    }
}
