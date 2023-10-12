using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAP_B1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Invoice());
        }
    }

    sealed class globalD
    {
        public static SAPbobsCOM.Company oCompany = new SAPbobsCOM.Company();
        public static SAPbobsCOM.Documents oInvoice; // Invoice Object
        public static string ErrMsg = "";
		public static int ErrCode = 0;
        public static string InvEntry;
        
    }

}
