using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
namespace DxAddIn
{
    public partial class DxTempTableViewer : UserControl
    {
        public DxTempTableViewer()
        {
            InitializeComponent();
        }
        public ToolStripLabel TsMsg { get { return tsmsg; } }
        private DataTable _dt;

        public DataTable Dt
        {
            get { return _dt; }
            set
            {
                _dt = value;
                dataGridView1.DataSource = _dt;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var start = toolStripTextBox1.Text.ToString();
            CreateDataInExcel(Dt, "current", start);
        }
        void CreateDataInExcel(DataTable dtresult, string type, string start)
        {
            try
            {
                int rows = dtresult.Rows.Count + 1;
                int cols = dtresult.Columns.Count;
                string[,] s = new string[rows, cols];
                for (int k = 0; k < cols; k++)
                {
                    s[0, k] = dtresult.Columns[k].ColumnName;
                }
                for (int i = 1; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        s[i, j] = dtresult.Rows[i - 1][j].ToString();
                    }
                }
                Excel.Worksheet ws;
                if (type.Equals("new"))
                {
                    ws = CurrentApp.App.ActiveWorkbook.Sheets.Add();
                }
                else
                {
                    ws = CurrentApp.App.ActiveSheet as Excel.Worksheet;
                }
                ws.Range[start].get_Resize(rows, cols).Value2 = s;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
    }
}
