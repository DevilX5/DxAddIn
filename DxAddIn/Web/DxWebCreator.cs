using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DxAddIn.Web.WebCreator;

namespace DxAddIn.Web
{
    public partial class DxWebCreator : UserControl
    {
        public DxWebCreator()
        {
            InitializeComponent();
            Wc = new WebCreator();
            Wc.Data = new List<ZhEn>();
        }
        public RichTextBox ModelRtb => rtbModel;
        public RichTextBox ViewRtb => rtbView;
        public RichTextBox ControllerRtb => rtbController;
        public RichTextBox DbRtb => rtbDb;
        private  string _cb;

        public string Cb
        {
            set
            {
                _cb = value;
                cbhv.Text = _cb;
            }
        }


        public WebCreator Wc { get; set; }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var hv = cbhv.Text;
            var range = ExcelHelper.SelectRange;
            Wc.Data=hv.Equals("纵向")? ExcelHelper.ObjArrToList<ZhEn>(range.Value): ExcelHelper.ObjArrToListV<ZhEn>(range.Value);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Wc.ClassName = toolStripTextBox1.Text;
            Wc.LabelPrefix = toolStripTextBox2.Text;
            Wc.ControllerName = toolStripTextBox3.Text;
            Wc.ExportName = toolStripTextBox4.Text;

            ModelRtb.Text = Wc.CreateClass();
            rtbServices.Text = Wc.CreateServices();
            ControllerRtb.Text = Wc.CreateController();
            ViewRtb.Text = Wc.CreatePage1();
            AnotherViewRtb.Text = Wc.CreatePage();
            rtbDb.Text = Wc.CreateDbStr();

        }
    }
}
