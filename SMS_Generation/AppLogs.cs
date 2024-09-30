using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using NACH_Application;

namespace SMS_Generation
{
    class AppLogs
    {

        public static logs logs = new logs();

        public static void WriteError(Exception e)
        {
            logs.Info("\n--------------------- Exception Date/Time" + DateTime.Now + "--------------------------------------\n");
            logs.Error("Exception : " + e.Message);
        }

        public static void WriteInfo(string info)
        {
            logs.Info("\n--------------------- Info Date/Time" + DateTime.Now + "--------------------------------------\n");
            logs.Info("\n " + "   Message " + info + "  -\n");

        }

    }


    public class DB_Logs
    {
        cls_DBConnection _dbobj = new cls_DBConnection();
        public string genUniqueno()
        {
            string MyNumber = "";
            MyNumber = DateTime.Now.ToString("ddMMyyyyHHmmssfff") + "" + Guid.NewGuid();
            return MyNumber;
        }

        public void DBError_Logs(string excemgs)
        {
            try
            {

                System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
                string LogEnable = ConfigurationManager.AppSettings["LogEnable"].ToString();
                if (LogEnable == "Y")
                {
                    string qry = "insert into applogs LogID,LogDate,Info,LogType values ('" + genUniqueno() + "' , now(), 'Exception : " + excemgs + "', 'EXCEPTION')";
                    _dbobj.gl_ExeNonQuery(qry);
                }
            }
            catch (Exception ex)
            {
                AppLogs.logs.Error("DBLog Exception : " + ex.Message);

            }
        }

        public void DBInfo_Logs(string info)
        {
            try
            {
                System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
                string LogEnable = ConfigurationManager.AppSettings["LogEnable"].ToString();
                if (LogEnable == "Y")
                {
                    string qry = "insert into applogs LogID,LogDate,Info,LogType values ('" + genUniqueno() + "' , now(), 'Info : " + info + "', 'EXCEPTION')";
                    _dbobj.gl_ExeNonQuery(qry);
                }
            }
            catch (Exception ex)
            {
                AppLogs.logs.Error("DBLog Exception : " + ex.Message);
            }
        }
    }
    public class logs
    {
        //System.Web.HttpContext.Current.Server.MapPath("ServiceLogs")+"\\"+        
        public void Info(string message)
        {
            try
            {
                string mLogPath = ConfigurationManager.AppSettings["LogPath"].ToString();
                System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
                string LogEnable = ConfigurationManager.AppSettings["LogEnable"].ToString();
                if (LogEnable == "Y")
                {
                    File.AppendAllText(LogPath(), Environment.NewLine);
                    File.AppendAllText(LogPath(), "Info :" + message);
                }
            }
            catch
            {

            }
        }

        public void Error(string errmessage)
        {
            try
            {
                string mLogPath = ConfigurationManager.AppSettings["LogPath"].ToString();
                System.Web.UI.WebControls.TreeNode node = new System.Web.UI.WebControls.TreeNode();
                string LogEnable = ConfigurationManager.AppSettings["LogEnable"].ToString();
                if (LogEnable == "Y")
                {
                    File.AppendAllText(LogPath(), Environment.NewLine);
                    File.AppendAllText(LogPath(), "Exception : + " + errmessage);
                }
            }
            catch
            {

            }
        }

        private String LogPath()
        {
            string mLogPath = ConfigurationManager.AppSettings["LogPath"].ToString();
            string mErrorLogPath = string.Empty;

            mErrorLogPath = mLogPath;
            if (!Directory.Exists(mErrorLogPath))
            {
                Directory.CreateDirectory(mErrorLogPath);
            }
            //Added Git Repo
            mErrorLogPath = mErrorLogPath + "UCO_SMS_Generation" + DateTime.Now.ToString("dd-MMM-yyyy HH") + ".txt";
            return mErrorLogPath;
        }
    }
}
