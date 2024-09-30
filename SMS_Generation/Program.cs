using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NACH_Application;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Net;  

namespace SMS_Generation
{
    class Program
    {
        cls_DBConnection DbObj = new cls_DBConnection();
        string m_str;
        static void Main(string[] args)
        {
            string filePath = ConfigurationManager.AppSettings["LogPath"].ToString();
            string logFile = filePath + "AutoSMSGenerationUCOLog_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            AppLogs.logs.Info("Path of Log and Name : "+ logFile);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            try
            {
                AppLogs.logs.Info("********************************************");
                AppLogs.logs.Info("UCO SMS Generation Auto Service started at : " + DateTime.Now);
                AppLogs.logs.Info("********************************************");

                Program _apprun = new Program();  
                _apprun.SMSGeneration();
            }
            catch (Exception err)
            {
                AppLogs.logs.Error("Application Error : " + err.Message);
                Environment.Exit(0);
            }
            finally
            {
                AppLogs.logs.Info("********************************************");
                AppLogs.logs.Info("UCO SMS Generation Auto Service Ended at : " + DateTime.Now);
                AppLogs.logs.Info("********************************************");
            }
        }
        public void SMSGeneration()
        {
            try
            {
                AppLogs.logs.Info("Auto Fetching SMS Data started at : " + DateTime.Now);
                string SMSTable = ConfigurationManager.AppSettings["SMSTable"].ToString();
                AppLogs.logs.Info("Name of the SMS Table :" + SMSTable);
                DataSet mDSet = null;
                m_str = " SELECT * FROM " + SMSTable + " WHERE SMS_STATUS = 'In-Process' AND SMS_DATE IS NULL AND MSGSENT_TIME IS NULL AND IHNO IS NOT NULL";
                AppLogs.logs.Info("Query to Fetched Data from SMS Table: " + m_str);
                mDSet = DbObj.gl_ExeDataSet(m_str);
                AppLogs.logs.Info("Data Fetch From SMS Table Successfully");
                for (int i = 0; i < mDSet.Tables[0].Rows.Count; i++)
                {
                    string mobile = mDSet.Tables[0].Rows[i]["MOBILE_NO"].ToString();
                    string message = mDSet.Tables[0].Rows[i]["MESSAGE"].ToString();
                    string Srno = mDSet.Tables[0].Rows[i]["SRNO"].ToString();
                    string umrn = mDSet.Tables[0].Rows[i]["UMRN"].ToString();
                    string ihno = mDSet.Tables[0].Rows[i]["IHNO"].ToString();
                    string smstype = mDSet.Tables[0].Rows[i]["SMS_TYPE"].ToString();
                    string txnsrno = mDSet.Tables[0].Rows[i]["TXNSRNO"].ToString();
                    AppLogs.logs.Info("Entering into sendSMS method with mobile : " + mobile + " and Message : " + message);
                    string result = sendSMS(mobile, message);
                    AppLogs.logs.Info("Code went through sendSMS method Successfully and result : "+ result);
                    if (result == "send")
                    {
                        string Rtnmsg = UpdateData(Srno, umrn, ihno, "Success", smstype, txnsrno);
                        AppLogs.logs.Info("Data Updated in the SMS Table with result success : " + Rtnmsg);
                    }
                    else
                    {
                        string Rtnmsg = UpdateData(Srno, umrn, ihno, "Failed", smstype, txnsrno);
                        AppLogs.logs.Info("Data Updated in the SMS Table with result failed : " + Rtnmsg);
                    }
                }
            }
            catch (Exception ex)
            {
                AppLogs.logs.Error("SMSGeneration method catch Error : " + ex.Message);
                Environment.Exit(0);
            }
        }

        public static string sendSMS(string mobile, string msg)
        {
            AppLogs.logs.Info("********************************************");
            AppLogs.logs.Info("Entered into SendSMS started at : " + DateTime.Now);
            AppLogs.logs.Info("********************************************");
            string filePath = ConfigurationManager.AppSettings["LogPath"].ToString();
            string logFile = filePath + "SendSMSLog_" + DateTime.Now.ToString("ddMMyyyy") + ".txt";
            AppLogs.logs.Info("Log Path with File Name : " + logFile);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            
            string result = "";
            string mob = mobile;
            AppLogs.logs.Info("Mobile Number in the sendSMS method : " + mob);
            string apimsgid = "";
            try
            {
                AppLogs.logs.Info("Sending SMS started at : " + DateTime.Now);

                //mob = (mob.Length > 10) ? mob : "91" + mob;

                AppLogs.logs.Info("Final mobile no : "+ mob + " at "+ DateTime.Now );

                ServicePointManager.DefaultConnectionLimit = 9999;
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
                System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                                                                    (se, cert, chain, sslerror) => { return true; };

                //String smsAuthKey = ConfigurationManager.AppSettings["SMSAuthKey"].ToString();
                //AppLogs.logs.Info("smsAuthKey :" + smsAuthKey);
                String url = ConfigurationManager.AppSettings["SMS_URL"].ToString();
                AppLogs.logs.Info("URL OF SMS API : " + url);
                String userid = ConfigurationManager.AppSettings["username"].ToString();
                AppLogs.logs.Info("User ID : " + userid);
                String password = ConfigurationManager.AppSettings["password"].ToString();
                AppLogs.logs.Info("Password : " + password);
                String Feedid = ConfigurationManager.AppSettings["Feedid"].ToString();
                AppLogs.logs.Info("Feedid : " + Feedid);

                string cardno = "";
                string Kioskid = "1234";
                // url = "https://enterprise.smsgupshup.com/GatewayAPI/rest?";

                Console.WriteLine(url + "  Web API new Request  URL  " + "");

                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                Random rnd = new Random();
                rnd.Next();
                long myRandomNo = rnd.Next(100000, 999999);
                AppLogs.logs.Info("myRandomNo : " + myRandomNo);
                string smsText = msg;
                AppLogs.logs.Info("smsText :" + smsText);
                smsText = "feedid=" + Feedid + "&username=" + userid + "&password=" + password + "&To=" + mobile + "&Text="+msg+".&time=&senderid=UCOBNK";
                AppLogs.logs.Info("Final smsText :"+ smsText);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                // request.ContentType = "application/text";
                // request.Headers.Add("auth_key", smsAuthKey);
                apimsgid = "798" + DateTime.Now.ToString("mmSSfff");
                AppLogs.logs.Info("apimsgid" + apimsgid);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(smsText);
                    AppLogs.logs.Info("SMS take write request : " + DateTime.Now);
                    streamWriter.Flush();
                    AppLogs.logs.Info("Streamwriter Flushed");
                    streamWriter.Close();
                    AppLogs.logs.Info("Streamwriter Closed");
                }
                System.Net.WebResponse response = request.GetResponse();
                AppLogs.logs.Info("SMS GetResponse: " + DateTime.Now);
                var streamReader = new StreamReader(response.GetResponseStream());
                result = streamReader.ReadToEnd();
                AppLogs.logs.Info("SMS streamReader");
                Console.WriteLine(url + "  Http Request API Response :  " + result);
                AppLogs.logs.Info("Http Request API Response url : " + url + " And result : "+ result);
                return "send";
            }
            catch (Exception er)
            {
                Console.WriteLine("sendSMS method Catch Response Exception :" + er.Message);
                AppLogs.logs.Info("sendSMS method Catch Response Exception : "+ er.Message);
                throw new ArgumentException(er.Message);
            }
        }
        public string UpdateData(string SRNO, string smsUmrn, string ihno, string sms_status, string smstype, string txnsrno)
        {
            string SMSTable = ConfigurationManager.AppSettings["SMSTable"].ToString();
            //string nachoutward = ConfigurationManager.AppSettings["NachOutward"].ToString();
            //string nachtxnoutward = ConfigurationManager.AppSettings["NachTxnOutward"].ToString();
            //string nachtxnInward = ConfigurationManager.AppSettings["NachTxnInward"].ToString();
            //string msg = "";
            string msg1 = "";
            string sms_date = DateTime.Now.ToShortDateString();
            string msg_sent_time = DateTime.Now.ToShortTimeString();
            string a_str = "UPDATE " + SMSTable + " SET SMS_STATUS = '" + sms_status + "',SMS_DATE = to_date('" + sms_date + "','dd/mm/yyyy'),MSGSENT_TIME = '" + msg_sent_time + "' where SRNO = '" + SRNO + "' AND UMRN = '" + smsUmrn + "' AND IHNO = '" + ihno + "'";
            AppLogs.logs.Info("Query To Update Status :" + a_str);
            int i = DbObj.gl_ExeNonQuery(a_str);
            AppLogs.logs.Info("Details updated to SMS TABLE of umrn" + smsUmrn);
            #region
            //if (smstype == "MMS_RMR")
            //{
            //    string B_str = "UPDATE " + nachoutward + " SET MSGSENT_DATE = to_date('" + sms_date + "','dd/mm/yyyy'),MSGSENT_TIME2 = '" + msg_sent_time + "' where UMRN = '" + smsUmrn + "' AND IHNO = '" + ihno + "'";
            //    int j = DbObj.gl_ExeNonQuery(B_str);
            //    AppLogs.logs.Info("Details updated to NachOutward_25 of UMRN = '" + smsUmrn + "' and IHNO = '" + ihno + "'");
            //}
            //else if (smstype == "MMS_RMS")
            //{
            //    string B_str = "UPDATE " + nachoutward + " SET MSGSENT_DATE2 = to_date('" + sms_date + "','dd/mm/yyyy'),MSGSENT_TIME2 = '" + msg_sent_time + "' where UMRN = '" + smsUmrn + "' AND IHNO = '" + ihno + "'";
            //    int j = DbObj.gl_ExeNonQuery(B_str);
            //    AppLogs.logs.Info("Details updated to NachOutward_25 of UMRN = '" + smsUmrn + "' and IHNO = '" + ihno + "'");
            //}
            //else if (smstype == "ACHDR_OUTTXN")
            //{
            //    string c_str = "UPDATE " + nachtxnoutward + " SET MSGSENT_DATE = to_date('" + sms_date + "','dd/mm/yyyy'),MSGSENT_TIME2 = '" + msg_sent_time + "' where UMRN = '" + smsUmrn + "' AND SRNO = '" + txnsrno + "'";
            //    int k = DbObj.gl_ExeNonQuery(c_str);
            //    AppLogs.logs.Info("Details updated to  NachtxnOutward_25 of UMRN = '" + smsUmrn + "' and IHNO = '" + ihno + "' for TXN SRNO = '" + txnsrno + "'");
            //}
            //else if (smstype == "ACHDR_INTXN")
            //{
            //    string c_str = "UPDATE " + nachtxnInward + " SET MSGSENT_DATE = to_date('" + sms_date + "','dd/mm/yyyy'),MSGSENT_TIME2 = '" + msg_sent_time + "' where UMRN = '" + smsUmrn + "' AND SRNO = '" + txnsrno + "'";
            //    int k = DbObj.gl_ExeNonQuery(c_str);
            //    AppLogs.logs.Info("Details updated to NachtxnInward_25 of UMRN = '" + smsUmrn + "' and IHNO = '" + ihno + "' for TXN SRNO = '" + txnsrno + "'");
            //}
            //else if (smstype == "ACHDR_OUTTXNT-2")
            //{
            //    string c_str = "UPDATE " + nachtxnoutward + " SET MSGSENT_DATE = to_date('" + sms_date + "','dd/mm/yyyy'),MSGSENT_TIME2 = '" + msg_sent_time + "' where UMRN = '" + smsUmrn + "' AND SRNO = '" + txnsrno + "'";
            //    int k = DbObj.gl_ExeNonQuery(c_str);
            //    AppLogs.logs.Info("Details updated to NachtxnOutward_25 of UMRN = '" + smsUmrn + "' and IHNO = '" + ihno + "' for TXN SRNO = '" + txnsrno + "'");
            //}
            #endregion
            msg1 = "Details Updated to SMS_TABLE_25 table";
            return msg1;
        }
    }
}
