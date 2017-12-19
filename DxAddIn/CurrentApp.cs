using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace DxAddIn
{
    public class CurrentApp
    {
        public static Excel.Application App { get { return Globals.ThisAddIn.Application; } }
    }
}
