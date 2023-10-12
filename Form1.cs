using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SAP_B1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P && e.Modifiers == Keys.Control)
            {
                crystalReportViewer1.PrintReport();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                var rpt = new billing();
                var user = "sa";
                var pwd = "Sb1@usri";
                rpt.SetDatabaseLogon(user, pwd);
                rpt.SetParameterValue("DocKey@", globalD.InvEntry);

                crystalReportViewer1.ReportSource = rpt;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
