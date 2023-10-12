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


    public partial class MainForm : Form
    {
        private SAPbobsCOM.Recordset oRecordSet;

       
        public void SetResults()
        {
            txtCusCode.Text = oRecordSet.Fields.Item(0).Value.ToString();
            txtCusName.Text = oRecordSet.Fields.Item(1).Value.ToString();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LogInForm frm = new LogInForm();

            //show log in dialog
            frm.ShowDialog();

            if (globalD.oCompany.Connected == false)
            {
                Application.Exit();
            }

            globalD.oItems = (SAPbobsCOM.Items)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems);
        }

        private void button3_Click(object sender, EventArgs e)
        {

            // Getting an initialized Recordset object
            oRecordSet = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            oRecordSet.DoQuery(textBox4.Text);
            SetResults();
        }

        private void btnPrev_Click_1(object sender, EventArgs e)
        {
            if (oRecordSet.BoF == false)
            {
                oRecordSet.MovePrevious();
                SetResults();
            }
        }

        private void btnNext_Click_1(object sender, EventArgs e)
        {
            if (oRecordSet.EoF == false)
            {
                oRecordSet.MoveNext();
            }
            // before setting the results check the
            // Recordset pointer again
            if (oRecordSet.EoF == false)
            {
                SetResults();
            }
        }
    }
}  

