
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace NACH_Application
{
    public class cls_DBConnection
    {
        //OracleConnection con = new OracleConnection("Data Source=ADROITTEST;User ID=ecs;Password=adroit11;");
        //ADODB.Connection cnNach = new ADODB.Connection();  
        //OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["NACH_DB"].ConnectionString);
        //OracleCommand cmd=new OracleCommand();
        private static string conString = ConfigurationManager.ConnectionStrings["NACH_DB"].ConnectionString;
        public static string oledbConString = "Provider=OraOLEDB.Oracle.1;Persist Security Info=True;" + conString;
        ADODB.Connection cnNach = new ADODB.Connection();
        OracleConnection con = new OracleConnection(conString);
        OracleCommand cmd = new OracleCommand();
        static bool IsConnected = false;
        //Timer ConKillTimer = new Timer();

        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    //if (cmd != null)
        //    //{
        //    //    cmd.Dispose();
        //    //    //cmd = null;
        //    //}
        //    if (IsConnected)
        //    {
        //        if (con != null)
        //        {
        //            if (con.State != ConnectionState.Closed)
        //            {
        //                con.Close();
        //                //con.Dispose();
        //                //con = null;
        //            }
        //        }
        //    }

        //}

        public cls_DBConnection()
        {
            //ConKillTimer.Interval = 60000;
            //ConKillTimer.Enabled = true;
            //ConKillTimer.Tick += new System.EventHandler<EventArgs>(this.timer1_Tick);

            if (IsConnected == false)
            {
                cmd.Connection = con;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    IsConnected = true;
                }
            }

        }

        ~cls_DBConnection()
        {
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
            if (con != null)
            {
                if (con.State != ConnectionState.Closed)
                {
                    con.Close();
                    con.Dispose();
                    cmd = null;
                }
            }
            if (cnNach != null)
            {
                if (cnNach.State != 0)
                {
                    cnNach.Close();
                    //con.Dispose();
                    //con = null;
                }
            }
        }

        public OracleConnection getCon()
        {
            cmd.Connection = con;
            if (con.State == ConnectionState.Closed)
                con.Open();
            //ConKillTimer.Enabled = true;
            return con;
        }

        public void gl_dbClose()
        {
            con.Close();
        }

        public int gl_ExeNonQuery(string m_str)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = m_str;
            //ConKillTimer.Enabled = true;
            int i = cmd.ExecuteNonQuery();
            return i;
        }

        public object gl_exeScalar(string m_str)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = m_str;
            //ConKillTimer.Enabled = true;
            object ob = cmd.ExecuteScalar();
            return ob;
        }

        public OracleDataReader gl_ExeReader(string m_str)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            cmd.Connection = con;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = m_str;
            //ConKillTimer.Enabled = true;
            OracleDataReader dr = cmd.ExecuteReader();
            return dr;
        }

        public DataSet gl_ExeDataSet(string m_str)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            //OracleCommand cmd1 = new OracleCommand();
            //cmd.Connection = con;
            //cmd.CommandType = CommandType.Text;
            //cmd.CommandText = m_str;
            //ConKillTimer.Enabled = true;
            OracleDataAdapter da = new OracleDataAdapter(m_str, con);
            DataSet ds = new DataSet();
            da.Fill(ds);
            da.Dispose();
            return ds;
        }

        public DataTable gl_ExeDataTable(string m_str)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            //ConKillTimer.Enabled = true;
            //cmd.Connection = con;
            //cmd.CommandType = CommandType.Text;
            //cmd.CommandText = m_str;
            OracleDataAdapter da = new OracleDataAdapter(m_str, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            da.Dispose();
            return dt;
        }

        public void gl_FillCombo(string m_str, DropDownList cbx, string m_TextField, string m_ValueField)
        {
            DataSet ds1 = new DataSet();
            cbx.DataTextField = m_TextField;
            cbx.DataValueField = m_ValueField;
            // cbx.SelectedValue = null;
            ds1 = gl_ExeDataSet(m_str);
            cbx.DataSource = ds1.Tables[0];
            cbx.DataBind();
        }

        public void gl_FillListBox(string m_str, ListBox lst, string m_TextField, string m_ValueField)
        {
            DataSet dslst = new DataSet();
            lst.DataTextField = m_TextField;
            lst.DataValueField = m_ValueField;
            dslst = gl_ExeDataSet(m_str);
            lst.DataSource = dslst.Tables[0];

        }

        public ADODB.Connection gl_AdodcConnection()
        {
            //cnNach.ConnectionString = "Provider=OraOLEDB.Oracle.1;Password=adroit11nach;Persist Security Info=True;User ID=NACH;Data Source=adroit"; //UAT
            //cnNach.ConnectionString = "Provider=OraOLEDB.Oracle.1;Password=adroit11;Persist Security Info=True;User ID=NACH;Data Source=xe"; //comment while publishing
            //cnNach.ConnectionString = ConfigurationManager.ConnectionStrings["cnNach"].ConnectionString;
            //cnNach.ConnectionString = "Provider=OraOLEDB.Oracle.1;Password=adroit11;Persist Security Info=True;User ID=NACH;Data Source=corpdb"; //while publishing 
            //cnNach.ConnectionString = "Provider=OraOLEDB.Oracle.1;Password=adroit11;Persist Security Info=True;User ID=NACH;Data Source=nachdb"; //for OBC
            cnNach.ConnectionString = ("Provider=OraOLEDB.Oracle.1;Persist Security Info=True;" + conString); //comment while publishing
            cnNach.Open();
            return cnNach;
        }

        public void pr_RdOnlyRcrdSt(out ADODB.Recordset rsRdOnly, string SQL_RdOnly)
        {
            rsRdOnly = new ADODB.Recordset();
            rsRdOnly.ActiveConnection = gl_AdodcConnection();
            rsRdOnly.CursorType = ADODB.CursorTypeEnum.adOpenForwardOnly;
            rsRdOnly.CursorLocation = ADODB.CursorLocationEnum.adUseClient;
            rsRdOnly.LockType = ADODB.LockTypeEnum.adLockReadOnly;
            rsRdOnly.Open(SQL_RdOnly);

        }

        public void gl_AdodcConnectionClose()
        {
            cnNach.Close();
        }

    }
}
