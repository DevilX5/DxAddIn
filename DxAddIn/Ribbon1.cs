using DxAddIn.Web;
using Microsoft.Office.Tools.Ribbon;
using NPinyin;
using RestSharp;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DxAddIn.Web.WebCreator;
using Excel = Microsoft.Office.Interop.Excel;

namespace DxAddIn
{
    public partial class Ribbon1
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private async void button2_Click(object sender, RibbonControlEventArgs e)
        {
            var tp = new DxTempTableViewer();
            var p = Globals.ThisAddIn.CustomTaskPanes.Add(tp, "转换");
            tp.Dock = DockStyle.Fill;
            p.Width = 600;
            p.Visible = true;
            var t = Task.Run(() =>
            {
                var dt = ExcelHelper.GetDataTable();
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
                    MessageBox.Show("请选择一个以上的单元格");
                }
                return dt;
            });
            await t;
            tp.Dt = t.Result;
            tp.TsMsg.Text = $"数据获取完毕,共计{t.Result.Rows.Count}行";


            //t.GetAwaiter().OnCompleted(() =>
            //{
            //    tp.Invoke((MethodInvoker)delegate
            //    {
            //        tp.Dt = t.Result;
            //        tp.TsMsg.Text = $"数据获取完毕,共计{t.Result.Rows.Count}行";
            //    });
            //});

            //Task.Run(() =>
            //{
            //    Task.WaitAll(t);
            //    tp.Invoke((MethodInvoker)delegate
            //    {
            //        tp.Dt = t.Result;
            //        tp.TsMsg.Text = $"数据获取完毕,共计{t.Result.Rows.Count}行";
            //    });
            //});
        }


        private void btnWebCreate_Click(object sender, RibbonControlEventArgs e)
        {
            var range = ExcelHelper.SelectRange;
            if (range != null)
            {
                var tp = new DxWebCreator();
                tp.Cb = "纵向";
                tp.Wc.Data= ExcelHelper.ObjArrToList<ZhEn>(range.Value);
                var p = Globals.ThisAddIn.CustomTaskPanes.Add(tp, "MVC生成器");
                tp.Dock = DockStyle.Fill;
                p.Width = 1000;
                p.Visible = true;
            }
        }

        private void button3_Click(object sender, RibbonControlEventArgs e)
        {
            var range = ExcelHelper.SelectRange;
            if (range != null)
            {
                Excel.Range f = ExcelHelper.App.InputBox("选择放置位置", Type: 8);
                var start = f.Address.Replace("$", "").Split(':')[0];
                var data = range.Cast<Excel.Range>().Select((s, i) =>
                {
                    return Pinyin.GetInitials(s.Value);
                }).ToList();

                var rows = range.Rows.Count;
                var cols = range.Columns.Count;
                var result = new object[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = data[i];
                    }
                }
                //ws.Range[start].get_Resize(rows, cols).Value2 = s;
                ExcelHelper.Worksheet.Range[start].get_Resize(range.Rows.Count, range.Columns.Count).Value = result;
            }
        }

        private void button1_Click(object sender, RibbonControlEventArgs e)
        {
            var range = ExcelHelper.SelectRange;
            if (range != null)
            {
                Excel.Range f = ExcelHelper.App.InputBox("选择放置位置", Type: 8);
                var start = f.Address.Replace("$", "").Split(':')[0];
                var data = range.Cast<Excel.Range>().Select((s, i) =>
                {
                    return Pinyin.GetPinyin(s.Value);
                }).ToList();

                var rows = range.Rows.Count;
                var cols = range.Columns.Count;
                var result = new object[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        result[i, j] = data[i];
                    }
                }
                //ws.Range[start].get_Resize(rows, cols).Value2 = s;
                ExcelHelper.Worksheet.Range[start].get_Resize(range.Rows.Count, range.Columns.Count).Value = result;
            }
        }

        private void btnWebCreateV_Click(object sender, RibbonControlEventArgs e)
        {
            var range = ExcelHelper.SelectRange;
            if (range != null)
            {
                var tp = new DxWebCreator();
                tp.Cb = "横向";
                tp.Wc.Data = ExcelHelper.ObjArrToListV<ZhEn>(range.Value);
                var p = Globals.ThisAddIn.CustomTaskPanes.Add(tp, "MVC生成器");
                tp.Dock = DockStyle.Fill;
                p.Width = 1000;
                p.Visible = true;
            }
        }

        private void button4_Click(object sender, RibbonControlEventArgs e)
        {

        }
       
    }
    
}
