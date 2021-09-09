using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using TKITDLL;

namespace TKCUSTOMERSERVICE
{
    public partial class frmCalls : Form
    {
        SqlConnection sqlConn = new SqlConnection();
        SqlCommand sqlComm = new SqlCommand();
        string connectionString;
        StringBuilder sbSql = new StringBuilder();
        StringBuilder sbSqlQuery = new StringBuilder();
        SqlDataAdapter adapter = new SqlDataAdapter();
        SqlCommandBuilder sqlCmdBuilder = new SqlCommandBuilder();
        SqlTransaction tran;
        SqlCommand cmd = new SqlCommand();
        DataSet ds = new DataSet();
        DataTable dt = new DataTable();
        string strFilePath;
        OpenFileDialog file = new OpenFileDialog();
        int result;
        string NowDay;
        int rownum = 0;

        public class CallsRecord
        {
            public string ID { set; get; }
            public string CallDate { set; get; }
            public string CallTime { set; get; }
            public string TypeID { set; get; }
            public string CallName { set; get; }
            public string CallPhone { set; get; }
            public string CallText { set; get; }
            public string CallTextRe { set; get; }
            public string  OrderID { set; get; }
            public string ShipID { set; get; }
            public string InvoiceNo { set; get; }
        }

        List<CallsRecord> list_CallsRecord = new List<CallsRecord>();


        public frmCalls()
        {
            InitializeComponent();
            comboboxload();
        }

        #region FUNCTION
        public void comboboxload()
        {

            //20210902密
            Class1 TKID = new Class1();//用new 建立類別實體
            SqlConnectionStringBuilder sqlsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString);

            //資料庫使用者密碼解密
            sqlsb.Password = TKID.Decryption(sqlsb.Password);
            sqlsb.UserID = TKID.Decryption(sqlsb.UserID);

            String connectionString;
            sqlConn = new SqlConnection(sqlsb.ConnectionString);

            String Sequel = "SELECT  [TypeID],[TypeName]  FROM [TKCUSTOMERSERVICE].[dbo].[BASETYPE]";
            adapter = new SqlDataAdapter(Sequel, sqlConn);
            dt = new DataTable();
            sqlConn.Open();

            dt.Columns.Add("TypeID", typeof(string));
            dt.Columns.Add("TypeName", typeof(string));
            adapter.Fill(dt);
            comboBox1.DataSource = dt.DefaultView;
            comboBox1.ValueMember = "TypeID";
            comboBox1.DisplayMember = "TypeName";
            sqlConn.Close();


        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var curRow = dataGridView1.CurrentRow;
            if (curRow != null)
            {
                textBox1.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                textBox2.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                textBox8.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();
                textBox3.Text = dataGridView1.CurrentRow.Cells[9].Value.ToString();
                textBox4.Text = dataGridView1.CurrentRow.Cells[10].Value.ToString();
                textBox5.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                textBox6.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
                textBox7.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
                comboBox1.Text= dataGridView1.CurrentRow.Cells[3].Value.ToString();

            }
        }
        public void Search()
        {
            try
            {

                if (!string.IsNullOrEmpty(dateTimePicker1.Text.ToString())&&!string.IsNullOrEmpty(dateTimePicker2.Text.ToString()))
                {
                    //20210902密
                    Class1 TKID = new Class1();//用new 建立類別實體
                    SqlConnectionStringBuilder sqlsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString);

                    //資料庫使用者密碼解密
                    sqlsb.Password = TKID.Decryption(sqlsb.Password);
                    sqlsb.UserID = TKID.Decryption(sqlsb.UserID);

                    String connectionString;
                    sqlConn = new SqlConnection(sqlsb.ConnectionString);


                    sbSql.Clear();
                    sbSqlQuery.Clear();

                    sbSqlQuery.AppendFormat("AND CallDate>='{0}' AND  CallDate<='{1}'", dateTimePicker1.Value.ToString("yyyyMMdd"), dateTimePicker2.Value.ToString("yyyyMMdd"));
                    sbSql.AppendFormat(@"SELECT [ID] AS '編號',[CallDate]  AS '來電日',[CallTime]  AS '來電時間',[TypeName] AS '類別',[CallName] AS '來電名稱',[CallPhone] AS '來電電話',[OrderID] AS '訂單',[ShipID] AS '出貨單',[InvoiceNo] AS '發票',[CallText] AS '問題',[CallTextRe] AS '回覆' FROM [{0}].[dbo].[CALLRECORD],[{1}].[dbo].[BASETYPE]   WHERE CALLRECORD.TypeID=BASETYPE.TypeID {2}  ", sqlConn.Database.ToString(), sqlConn.Database.ToString(), sbSqlQuery.ToString());

                    adapter = new SqlDataAdapter(@"" + sbSql, sqlConn);
                    sqlCmdBuilder = new SqlCommandBuilder(adapter);

                    sqlConn.Open();
                    ds.Clear();
                    adapter.Fill(ds, "TEMPds");
                    sqlConn.Close();

                    label1.Text = "資料筆數:" + ds.Tables["TEMPds"].Rows.Count.ToString();

                    if (ds.Tables["TEMPds"].Rows.Count == 0)
                    {
                        
                    }
                    else
                    {                        
                        dataGridView1.DataSource = ds.Tables["TEMPds"];
                        dataGridView1.AutoResizeColumns();
                        rownum = ds.Tables["TEMPds"].Rows.Count - 1;
                        dataGridView1.CurrentCell = dataGridView1.Rows[rownum].Cells[0];
                        textBox5.Text = dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                        //dataGridView1.CurrentCell = dataGridView1[0, 2];

                    }
                }
                else
                {

                }



            }
            catch
            {

            }
            finally
            {

            }

        }

        public void ADDtoDB()
        {


            list_CallsRecord.Clear();
            list_CallsRecord.Add(new CallsRecord() { ID = textBox5.Text.ToString(), CallDate= DateTime.Now.ToString("yyyyMMdd"), CallTime= DateTime.Now.ToString("HH:mm:ss"), TypeID= comboBox1.SelectedValue.ToString(), CallName= textBox1.Text.ToString(), CallPhone= textBox2.Text.ToString(),OrderID=textBox6.Text.ToString() ,ShipID = textBox7.Text.ToString(), InvoiceNo=textBox8.Text.ToString(), CallText= textBox3.Text.ToString(), CallTextRe= textBox4.Text.ToString() });

            try
            {

                //20210902密
                Class1 TKID = new Class1();//用new 建立類別實體
                SqlConnectionStringBuilder sqlsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString);

                //資料庫使用者密碼解密
                sqlsb.Password = TKID.Decryption(sqlsb.Password);
                sqlsb.UserID = TKID.Decryption(sqlsb.UserID);

                String connectionString;
                sqlConn = new SqlConnection(sqlsb.ConnectionString);


                sqlConn.Close();
                sqlConn.Open();
                tran = sqlConn.BeginTransaction();

                sbSql.Clear();
                //sbSql.Append("UPDATE Member SET Cname='009999',Mobile1='009999',Telphone='',Email='',Address='',Sex='',Birthday='' WHERE ID='009999'");

                sbSql.AppendFormat(" INSERT INTO  [{0}].[dbo].[CALLRECORD] ([CallDate],[CallTime],[TypeID],[CallName],[CallPhone],[OrderID],[ShipID],[InvoiceNo],[CallText],[CallTextRe])  VALUES ('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}') ", sqlConn.Database.ToString(), list_CallsRecord[0].CallDate.ToString(), list_CallsRecord[0].CallTime.ToString(), list_CallsRecord[0].TypeID.ToString(), list_CallsRecord[0].CallName.ToString(), list_CallsRecord[0].CallPhone.ToString(), list_CallsRecord[0].OrderID.ToString(), list_CallsRecord[0].ShipID.ToString(), list_CallsRecord[0].InvoiceNo.ToString(), list_CallsRecord[0].CallText.ToString(), list_CallsRecord[0].CallTextRe.ToString());
                //sbSql.AppendFormat("  UPDATE Member SET Cname='{1}',Mobile1='{2}' WHERE ID='{0}' ", list_Member[0].ID.ToString(), list_Member[0].Cname.ToString(), list_Member[0].Mobile1.ToString());

                cmd.Connection = sqlConn;
                cmd.CommandTimeout = 60;
                cmd.CommandText = sbSql.ToString();
                cmd.Transaction = tran;
                result = cmd.ExecuteNonQuery();

                if (result == 0)
                {
                    tran.Rollback();    //交易取消
                }
                else
                {
                    tran.Commit();      //執行交易

                }

                sqlConn.Close();

                rownum = dataGridView1.RowCount;

                Search();

                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox6.Clear();
                textBox7.Clear();
                textBox8.Clear();
            }
            catch
            {

            }

            finally
            {
                sqlConn.Close();
            }

        }

        public void UpdateDB()
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("是否真的要更新", "UPDATE?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //20210902密
                    Class1 TKID = new Class1();//用new 建立類別實體
                    SqlConnectionStringBuilder sqlsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString);

                    //資料庫使用者密碼解密
                    sqlsb.Password = TKID.Decryption(sqlsb.Password);
                    sqlsb.UserID = TKID.Decryption(sqlsb.UserID);

                    String connectionString;
                    sqlConn = new SqlConnection(sqlsb.ConnectionString);


                    sqlConn.Close();
                    sqlConn.Open();
                    tran = sqlConn.BeginTransaction();

                    sbSql.Clear();
                    //sbSql.Append("UPDATE Member SET Cname='009999',Mobile1='009999',Telphone='',Email='',Address='',Sex='',Birthday='' WHERE ID='009999'");

                    sbSql.AppendFormat("UPDATE [{0}].dbo.[CALLRECORD]   SET [TypeID]='{2}',[CallName]='{3}',[CallPhone]='{4}',[CallText]='{5}',[CallTextRe]='{6}',[OrderID]='{7}',[ShipID]='{8}',[InvoiceNo]='{9}' WHERE [ID]='{1}' ", sqlConn.Database.ToString(), textBox5.Text.ToString(), comboBox1.SelectedValue.ToString(), textBox1.Text.ToString(), textBox2.Text.ToString(), textBox3.Text.ToString(), textBox4.Text.ToString(), textBox6.Text.ToString(), textBox7.Text.ToString(), textBox8.Text.ToString());
                    //sbSql.AppendFormat("  UPDATE Member SET Cname='{1}',Mobile1='{2}' WHERE ID='{0}' ", list_Member[0].ID.ToString(), list_Member[0].Cname.ToString(), list_Member[0].Mobile1.ToString());

                    cmd.Connection = sqlConn;
                    cmd.CommandTimeout = 60;
                    cmd.CommandText = sbSql.ToString();
                    cmd.Transaction = tran;
                    result = cmd.ExecuteNonQuery();

                    if (result == 0)
                    {
                        tran.Rollback();    //交易取消
                    }
                    else
                    {
                        tran.Commit();      //執行交易
                    }

                    sqlConn.Close();

                    Search();
                }

            }
            catch
            {

            }

            finally
            {
                sqlConn.Close();
            }
        }

        public void ClearText()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
        }
        public void DelDB()
        {
            try
            {
               textBox5.Text= dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString();
                DialogResult dialogResult = MessageBox.Show("是否真的要刪除", "del?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //20210902密
                    Class1 TKID = new Class1();//用new 建立類別實體
                    SqlConnectionStringBuilder sqlsb = new SqlConnectionStringBuilder(ConfigurationManager.ConnectionStrings["dbconn"].ConnectionString);

                    //資料庫使用者密碼解密
                    sqlsb.Password = TKID.Decryption(sqlsb.Password);
                    sqlsb.UserID = TKID.Decryption(sqlsb.UserID);

                    String connectionString;
                    sqlConn = new SqlConnection(sqlsb.ConnectionString);


                    sqlConn.Close();
                    sqlConn.Open();
                    tran = sqlConn.BeginTransaction();

                    sbSql.Clear();
                    //sbSql.Append("UPDATE Member SET Cname='009999',Mobile1='009999',Telphone='',Email='',Address='',Sex='',Birthday='' WHERE ID='009999'");

                    sbSql.AppendFormat("DELETE [{0}].dbo.[CALLRECORD] WHERE ID='{1}' ", sqlConn.Database.ToString(), textBox5.Text.ToString());
                    //sbSql.AppendFormat("  UPDATE Member SET Cname='{1}',Mobile1='{2}' WHERE ID='{0}' ", list_Member[0].ID.ToString(), list_Member[0].Cname.ToString(), list_Member[0].Mobile1.ToString());

                    cmd.Connection = sqlConn;
                    cmd.CommandTimeout = 60;
                    cmd.CommandText = sbSql.ToString();
                    cmd.Transaction = tran;
                    result = cmd.ExecuteNonQuery();

                    if (result == 0)
                    {
                        tran.Rollback();    //交易取消
                    }
                    else
                    {
                        tran.Commit();      //執行交易
                    }

                    sqlConn.Close();

                    Search();
                }
               
            }
            catch
            {

            }

            finally
            {
                sqlConn.Close();
            }

        }
        #endregion

        #region BUTTON
        private void button1_Click(object sender, EventArgs e)
        {
            Search();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ADDtoDB();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateDB();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DelDB();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            ClearText();
        }



        #endregion


    }
}
