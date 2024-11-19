using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAP_B1
{
    public partial class LogInForm : Form
    {
        public LogInForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Create a new company object

            int lRetCode;
            int lErrCode = 0;
            string sErrMsg = "";

            //// Set the mandatory properties for the connection to the database.
            //// To use a remote Db Server enter his name instead of the string "(local)"
            //// This string is used to work on a DB installed on your local machine
            try
            {

                globalD.oCompany.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_MSSQL2014; // MSSQLVersion
                globalD.oCompany.Server = "serverhostname";
                globalD.oCompany.language = SAPbobsCOM.BoSuppLangs.ln_English;
                globalD.oCompany.UseTrusted = false;
                globalD.oCompany.CompanyDB = "databasename";
                globalD.oCompany.DbUserName = "sa";
                globalD.oCompany.DbPassword = "pass";

                globalD.oCompany.UserName = textUserName.Text;
                //user password
                globalD.oCompany.Password = textPassword.Text;

                //Change mouse cursor
                Cursor = System.Windows.Forms.Cursors.WaitCursor;
                //Connecting to a company DB
                lRetCode = globalD.oCompany.Connect();

                if (lRetCode != 0)
                {
                    int temp_int = lErrCode;
                    string temp_string = sErrMsg;
                    globalD.oCompany.GetLastError(out temp_int, out temp_string);
                    MessageBox.Show(temp_string);
                    globalD.oCompany.Disconnect();
                    return;
                }

                //Change mouse cursor
                Cursor = System.Windows.Forms.Cursors.Default;

                this.Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);

            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
