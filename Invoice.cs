using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using SAPbobsCOM;


namespace SAP_B1
{
    public partial class Invoice : Form
    {

        public Invoice()
        {
            InitializeComponent();
        }

        SAPbobsCOM.Recordset rs;
        string drlist;
        string drDownPayment;
        string customercode;
        string sSeries;
        string drdate;
        string getdr;
        string DrNo;
       string withHoldingTax;


        private void drnumber()
        {
            string sSQL = "select distinct T3.DocEntry'Id' ,convert(varchar, T3.DocDAte, 107) 'Date' , T3.DocEntry, T3.U_DRNo ,T3.CardCode , T3.CardName, FORMAT(T3.DocTotal,'#,##0.00') from ordr T0 INNER join rdr1 T1 on T1.DocEntry = T0.DocEntry INNER JOIN DLN1 T2 ON T1.DocEntry = T2.BaseEntry AND T1.LineNum = T2.BaseLine AND T1.ObjType = T2.BaseType INNER JOIN ODLN T3 ON T2.DocEntry = T3.DocEntry where T0.CANCELED <> 'Y' and T3.CANCELED <> 'Y' and T0.DocStatus = 'C' AND T3.DocStatus = 'O' AND T0.DocEntry = '{0}'";
            sSQL = string.Format(sSQL, getdr);
            rs = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);

            dataGridView2.Rows.Clear();
            var collection = new List<string>();
            var DrNos = new List<string>();
            while (!(rs.EoF == true))
            {
                //// make sure the record set hasn't reach the EOF
            
                collection.Add(rs.Fields.Item(0).Value.ToString());
                drdate = rs.Fields.Item(1).Value.ToString();
                DrNos.Add(rs.Fields.Item(3).Value.ToString());
                customercode = rs.Fields.Item(4).Value.ToString();
                var row = new string[]
                    {
                    rs.Fields.Item(0).Value.ToString(),
                    rs.Fields.Item(1).Value.ToString(),
                    rs.Fields.Item(2).Value.ToString(),
                    rs.Fields.Item(3).Value.ToString(),
                    rs.Fields.Item(4).Value.ToString(),
                    rs.Fields.Item(5).Value.ToString(),
                    rs.Fields.Item(6).Value.ToString()

                    };
                dataGridView2.Rows.Add(row);

                rs.MoveNext();
            }
            drlist = string.Join(",", collection.ToArray());
            DrNo = string.Join(", ", DrNos.ToArray());



        }
        private void DonwPaymentInvoice()
        {
        

            string sSQL = " select distinct T3.DocEntry from ordr T0 INNER join rdr1 T1 on T1.DocEntry = T0.DocEntry INNER JOIN DPI1 T2 ON T1.DocEntry = T2.BaseEntry AND T1.LineNum = T2.BaseLine AND T1.ObjType = T2.BaseType INNER JOIN ODPI T3 ON T2.DocEntry = T3.DocEntry where T3.CANCELED <> 'Y' AND T0.DocEntry = '{0}'";
            sSQL = string.Format(sSQL, getdr);
            rs = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);
          
            var collection2 = new List<string>();
            while (!(rs.EoF == true))
            {
                if (rs.Fields.Item(0).Value != null)
                {

                    //// make sure the record set hasn't reach the EOF
                    collection2.Add(rs.Fields.Item(0).Value.ToString());
                }
                rs.MoveNext();
                
            }
            drDownPayment = string.Join(",", collection2.ToArray());
        }

        private void CreateInvoice()
        {

            // Init the invoice object.
            globalD.oInvoice = (SAPbobsCOM.Documents)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInvoices);
           //Header in Invoice
                globalD.oInvoice.CardCode = customercode;
                globalD.oInvoice.DocDate = dateTimePicker1.Value;
                globalD.oInvoice.UserFields.Fields.Item("U_PrepBy").Value = txtPreparedBy.Text;
                globalD.oInvoice.UserFields.Fields.Item("U_AppBy").Value = "Linlloyd Lau";
                globalD.oInvoice.UserFields.Fields.Item("U_Territory").Value = cmbTerritory.Text;
                globalD.oInvoice.UserFields.Fields.Item("U_BSNo").Value = txtBilling.Text;
                globalD.oInvoice.UserFields.Fields.Item("U_DRNo").Value = DrNo;

            if (checkBox1.Checked)
            {
                globalD.oInvoice.WithholdingTaxData.WTCode = "CWT1";
                globalD.oInvoice.WithholdingTaxData.WTAmount = double.Parse(txtWithHoldingTax.Text);
            }
            

            // Create a Recordset object.
            string[] drlistArray = drlist.Split(',');
            int[] deliveryNotes = Array.ConvertAll(drlistArray, s => int.Parse(s));


            foreach (int deliveryNote in deliveryNotes)
            {

                SAPbobsCOM.Recordset oRecordSet = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                // Set the SQL query to count the number of delivery note lines.
                oRecordSet.DoQuery("select LineNum from dln1 where LineStatus = 'O' and DocEntry in (" + deliveryNote + ")");


                while (!(oRecordSet.EoF == true))
                {
                    // Set the BaseLine, BaseEntry, and BaseType properties on the invoice line.
                    globalD.oInvoice.Lines.BaseLine = oRecordSet.Fields.Item(0).Value;
                    globalD.oInvoice.Lines.BaseEntry = deliveryNote;
                    globalD.oInvoice.Lines.BaseType = System.Convert.ToInt32(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
                    globalD.oInvoice.Lines.VatGroup = "OT1";

                    globalD.oInvoice.Lines.Add();
                    oRecordSet.MoveNext();
                }


                // Set the SQL query to count the number of delivery note lines.
                string  sSQL = "select LineNum from dln3 where DocEntry in (" + deliveryNote + ")";
                rs = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rs.DoQuery(sSQL);
                while (!(rs.EoF == true))
                {
                    if (rs.Fields.Item(0).Value != null)
                    {

                        globalD.oInvoice.Expenses.BaseDocLine = rs.Fields.Item(0).Value;
                        globalD.oInvoice.Expenses.BaseDocEntry = deliveryNote;
                        globalD.oInvoice.Expenses.BaseDocType = System.Convert.ToInt32(SAPbobsCOM.BoObjectTypes.oDeliveryNotes);
                        globalD.oInvoice.Expenses.VatGroup = "OTNA";
                        globalD.oInvoice.Expenses.Add();
                    }
                    rs.MoveNext();

                }

            }

            if (drDownPayment != null && !string.IsNullOrEmpty(drDownPayment))
            {
                string[] drDownPaymentArray = drDownPayment.Split(',');
                int[] DownPayments = Array.ConvertAll(drDownPaymentArray, s => int.Parse(s));

                // Iterate over the DownPayments array and add the Invoice with Down Payment to SAPbobsCOM
                foreach (int DownPayment in DownPayments)
                {
                    SAPbobsCOM.DownPaymentsToDraw dpToDraw;
                    dpToDraw = globalD.oInvoice.DownPaymentsToDraw;
                    dpToDraw.DocEntry = DownPayment;
                }
            }


            // Add the invoice.
            if (globalD.oInvoice.Add() != 0)
                {
                    MessageBox.Show(globalD.oCompany.GetLastErrorDescription());
                }
                else
                {

                    MessageBox.Show("Added invoice");

                if (txtBilling.Text != "")
                {
                    billing();
                }

            }          
        }

        public void getdata2()
        {
            
            SAPbobsCOM.Recordset rs;
            string sSQL = "select distinct T0.DocEntry'Id' ,convert(varchar, T0.DocDAte, 107) 'Date' ,T0.U_SCPNo ,T0.CardCode , T0.CardName, format(T0.DocTotal, '#,##0.00') from ordr T0 INNER join rdr1 T1 on T1.DocEntry = T0.DocEntry INNER JOIN DLN1 T2 ON T1.DocEntry = T2.BaseEntry AND T1.LineNum = T2.BaseLine AND T1.ObjType = T2.BaseType INNER JOIN ODLN T3 ON T2.DocEntry = T3.DocEntry where T0.CANCELED <> 'Y' and T3.CANCELED <> 'Y' and T0.DocStatus = 'C' AND T3.DocStatus = 'O' AND T0.Series = '{0}' AND T0.U_SCPNo like '%{1}%' order by T0.DocEntry";
            sSQL = string.Format(sSQL, sSeries, txtScNo.Text);
            rs = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);

            // setting the Returned Results

            dataGridView1.Rows.Clear();
            while (!(rs.EoF == true))
            {
                
                //// make sure the record set hasn't reach the EOF
                string[] row = {
                    rs.Fields.Item(0).Value.ToString(),
                    rs.Fields.Item(1).Value.ToString(),
                    rs.Fields.Item(2).Value.ToString(),
                    rs.Fields.Item(3).Value.ToString(),
                    rs.Fields.Item(4).Value.ToString(),
                    rs.Fields.Item(5).Value.ToString()

                    };
                dataGridView1.Rows.Add(row);
                rs.MoveNext();
            }
        }

        public void getdata()
        {
            SAPbobsCOM.Recordset rs;
            string sSQL = "select distinct T0.DocEntry'Id' ,convert(varchar, T0.DocDAte, 107) 'Date' ,T0.U_SCPNo ,T0.CardCode , T0.CardName, format(T0.DocTotal, '#,##0.00') from ordr T0 INNER join rdr1 T1 on T1.DocEntry = T0.DocEntry INNER JOIN DLN1 T2 ON T1.DocEntry = T2.BaseEntry AND T1.LineNum = T2.BaseLine AND T1.ObjType = T2.BaseType INNER JOIN ODLN T3 ON T2.DocEntry = T3.DocEntry where T0.CANCELED <> 'Y' and T3.CANCELED <> 'Y' and T0.DocStatus = 'C' AND T3.DocStatus = 'O' AND T0.Series = '{0}' order by T0.DocEntry";
            sSQL = string.Format(sSQL, sSeries);
            rs = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);

 
            // setting the Returned Results

            dataGridView1.Rows.Clear();

            while (!(rs.EoF == true))
              

            {
                //// make sure the record set hasn't reach the EOF
                string[] row = {
                    rs.Fields.Item(0).Value.ToString(),
                    rs.Fields.Item(1).Value.ToString(),
                    rs.Fields.Item(2).Value.ToString(),
                    rs.Fields.Item(3).Value.ToString(),
                    rs.Fields.Item(4).Value.ToString(),
                    rs.Fields.Item(5).Value.ToString()
                  
                    };
                dataGridView1.Rows.Add(row);
     
                rs.MoveNext();
            }         
        }
  
        private void button2_Click(object sender, EventArgs e)
        {
            getdata();
        }    
        private void Invoice_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;
            txtScNo.Enabled = false;
     
            LogInForm frm = new LogInForm();

            //show log in dialog
            frm.ShowDialog();

            if (globalD.oCompany.Connected == false)
            {
                ProjectData.EndApp();
            }
            
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (txtPreparedBy.Text == "")
            { MessageBox.Show("Prepared by is required"); }
            else
            {
                dataGridView1.Rows.Clear();
                dataGridView2.Rows.Clear();
                CreateInvoice();
                txtBilling.Text = "";
                txtScNo.Text = "";
                getdata();
                drnumber();
                button3.Enabled = false;
        
            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
         
            getdata();
            txtScNo.Enabled = true;
            txtScNo.Text = "";
            label4.Text = "Total Number of Records : "+ dataGridView1.Rows.Count.ToString();
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
            button3.Enabled = true;
            if (e.RowIndex >= 0)
            {
                //gets a collection that contains all the rows
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                //populate the textbox from specific value of the coordinates of column and row.
               getdr = row.Cells[0].Value.ToString();
               withHoldingTax = row.Cells[5].Value.ToString();
            }
             drnumber();
             DonwPaymentInvoice();

            double result, withHolTax;
            result = double.Parse(withHoldingTax);

            withHolTax = result / 1.12 * 1/100;

            string formattedNumber = string.Format("{0:N2}", withHolTax);

            txtWithHoldingTax.Text = formattedNumber;
            dateTimePicker1.Text = drdate;
            
            label4.Text = "Total Number of Records : " + dataGridView1.Rows.Count.ToString();
        }

        private void cmbTerritory_SelectedIndexChanged(object sender, EventArgs e)
        {
            button3.Enabled = false;
            txtScNo.Enabled = false;
            txtScNo.Text = "";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            if (cmbTerritory.SelectedItem.ToString() == "Head Office")
            {
                sSeries = "99";
            }
            else if (cmbTerritory.SelectedItem.ToString() == "Pampanga")
            {
                sSeries = "97";
            }
            else if (cmbTerritory.SelectedItem.ToString() == "Pangasinan")
            {
                sSeries = "100";
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        public void billing()
        {
            SAPbobsCOM.Recordset rs;
            string sSQL = "SELECT max(DocEntry) FROM dbo.OINV where U_PrepBy = '{0}'";
            sSQL = string.Format(sSQL, txtPreparedBy.Text);
            rs = (SAPbobsCOM.Recordset)globalD.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            rs.DoQuery(sSQL);
        
            globalD.InvEntry = System.Convert.ToInt32(rs.Fields.Item(0).Value).ToString();
            
            Form1 frm = new Form1();
            frm.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            billing();
        }

        private void txtScNo_TextChanged(object sender, EventArgs e)
        {

            getdata2();
        }

        
    }
}
