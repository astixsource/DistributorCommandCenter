using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

/// <summary>
/// Summary description for clsSendLogMail
/// </summary>
public class clsSendLogMail
{
    // 
    //private static Hashtable _simpleTable;
    public clsSendLogMail()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    
    public static void fnSendLogMail(string strErrorMsg, string strErrorDescr, string strSourcePage, string strFunctionName, string strSubject, List<SqlParameter> sp = null)
    {
        StringBuilder mailString = new StringBuilder();
        //StringBuilder callDefinition = new StringBuilder();
        //if (sp!=null)
        //{
        //    callDefinition.Append(string.Format("ExecuteStoredProc: {0} (", strFunctionName));
        //    int i = 0;
        //    foreach (SqlParameter item in sp)
        //    {
        //        callDefinition.Append(string.Format("{0}={1}", item.ParameterName, item.Value));
        //        if (i < sp.Count() - 1)
        //        {
        //            callDefinition.Append(",");
        //        }
        //        i++;
        //    }
        //    callDefinition.Append(")");
        //}

        MailMessage objmail = new MailMessage();
        objmail = new System.Net.Mail.MailMessage();
        objmail.Subject = strSubject;
        mailString.Append("<font  style='COLOR: #000080; FONT-FAMILY: Arial'  size=2>");
        mailString.Append("<p>Dear, </p>");

        mailString.Append("<p>Kindly check and correct below error :--</p></br>");
        mailString.Append("<p>Source Page : " + strSourcePage + "</p>");
        mailString.Append("<p>Function Name : " + strFunctionName + "</p></br>");
        mailString.Append("<p>SQl Command Descr : </p>");
        mailString.Append("<p>LoginId : " + (HttpContext.Current.Session["LoginId"] == null ? "0" : HttpContext.Current.Session["LoginId"].ToString()) + "</p>");
        mailString.Append("<p>Error Name : " + strErrorMsg + "</p></br>");
        mailString.Append("<p>Error Description : " + strErrorDescr + "</p>");
        
        mailString.Append("<br><br><p>All the Best!</p>");
        mailString.Append("<br>Helpdesk");
        mailString.Append("<p>DMS</p>");
        mailString.Append("</font>");

        string errTo = ConfigurationSettings.AppSettings["errTo"];
        objmail.To.Add(errTo);

        string errCc = ConfigurationSettings.AppSettings["errCc"];
        if (!string.IsNullOrEmpty(errCc))
        {
            objmail.CC.Add(errCc);
        }

        string errBCc = ConfigurationSettings.AppSettings["errBCc"];
        if (!string.IsNullOrEmpty(errBCc))
        {
            objmail.Bcc.Add(errBCc);
        }

        objmail.IsBodyHtml = true;
        objmail.Body = Convert.ToString(mailString);
objmail.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["Title"] + "<astix@astixsolutions.com>");

        SmtpClient SmtpMail;
        SmtpMail = new SmtpClient();

        SmtpMail.Host = ConfigurationSettings.AppSettings["MailServerString"];
SmtpMail.Port =Convert.ToInt32(ConfigurationSettings.AppSettings["smtpPort"]);

        NetworkCredential loginInfo;
        loginInfo = new NetworkCredential();
        loginInfo.UserName = ConfigurationSettings.AppSettings["DMSUser"];
        loginInfo.Password = ConfigurationSettings.AppSettings["DMSGPassword"];
        SmtpMail.Credentials = loginInfo;
        SmtpMail.EnableSsl = true;
        SmtpMail.Timeout = 0;
        try
        {
            SmtpMail.Send(objmail);
            mailString = null;
        }
        catch (Exception ex)
        {

        }

    }

    public static void fnSendLogMailLog(string strErrorMsg, string strErrorDescr, string strSourcePage, string strFunctionName, string strSubject, string callDefinition = "")
    {
        StringBuilder mailString = new StringBuilder();
        MailMessage objmail = new MailMessage();
        objmail = new System.Net.Mail.MailMessage();
        objmail.Subject = strSubject;
        mailString.Append("<font  style='COLOR: #000080; FONT-FAMILY: Arial'  size=2>");
        mailString.Append("<p>Dear, </p>");

        mailString.Append("<p>Kindly check and correct below error :--</p></br>");
        mailString.Append("<p>Source Page : " + strSourcePage + "</p>");
        mailString.Append("<p>Function Name : " + strFunctionName + "</p></br>");
        mailString.Append("<p>SQl Command Descr : " + callDefinition + "</p>");
        mailString.Append("<p>LoginId : " + (HttpContext.Current.Session["LoginId"] == null ? "0" : HttpContext.Current.Session["LoginId"].ToString()) + "</p>");
        mailString.Append("<p>Error Name : " + strErrorMsg + "</p></br>");
        mailString.Append("<p>Error Description : " + strErrorDescr + "</p>");
        mailString.Append("<br><br><p>All the Best!</p>");
        mailString.Append("<br>Helpdesk");
        mailString.Append("<p>DMS</p>");
        mailString.Append("</font>");

        string errTo = ConfigurationSettings.AppSettings["errTo"];
        objmail.To.Add(errTo);

        string errCc = ConfigurationSettings.AppSettings["errCc"];
        if (!string.IsNullOrEmpty(errCc))
        {
            objmail.CC.Add(errCc);
        }

        string errBCc = ConfigurationSettings.AppSettings["errBCc"];
        if (!string.IsNullOrEmpty(errBCc))
        {
            objmail.Bcc.Add(errBCc);
        }

        objmail.IsBodyHtml = true;
        objmail.Body = Convert.ToString(mailString);
        objmail.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["Title"] + "<astix@astixsolutions.com>");

        SmtpClient SmtpMail;
        SmtpMail = new SmtpClient();

        SmtpMail.Host = ConfigurationSettings.AppSettings["MailServerString"];
SmtpMail.Port =Convert.ToInt32(ConfigurationSettings.AppSettings["smtpPort"]);

        NetworkCredential loginInfo;
        loginInfo = new NetworkCredential();
        loginInfo.UserName = ConfigurationSettings.AppSettings["DMSUser"];
        loginInfo.Password = ConfigurationSettings.AppSettings["DMSGPassword"];
        SmtpMail.Credentials = loginInfo;
        SmtpMail.EnableSsl = true;
        SmtpMail.Timeout = 0;
        try
        {
            SmtpMail.Send(objmail);
            mailString = null;
        }
        catch (Exception ex)
        {

        }

    }
    public static void SendErrorMail(string msg, string sFileName)
    {
        try
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("astix@astixsolutions.com");
            mail.To.Add(System.Configuration.ConfigurationManager.AppSettings["errTo"].ToString());
            mail.Subject = "Error in File Uploading App " + sFileName + " data upload) : " + DateTime.Now.ToString("dd-MMM-yyyy");

            mail.Body = msg;
            mail.IsBodyHtml = true;

            //mail.Attachments.Add(new Attachment(fileName));
            SmtpServer.Port =Convert.ToInt32(ConfigurationSettings.AppSettings["smtpPort"]);
            SmtpServer.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["DMSUser"].ToString(), System.Configuration.ConfigurationManager.AppSettings["DMSGPassword"].ToString());
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
        catch (Exception ex) { }
    }

    public static void fnSendMail(string subject,string emaild, string emailCC, string emailBody)
    {
        StringBuilder mailString = new StringBuilder();
        MailMessage objmail = new MailMessage();
        objmail = new System.Net.Mail.MailMessage();
        objmail.Subject = subject;
        mailString.Append("<font  style='COLOR: #000080; FONT-FAMILY: Arial'  size=2>");

        mailString.Append("<p>"+ emailBody + "</p>");
        mailString.Append("<br>Helpdesk");
        mailString.Append("<p>Astix</p>");
        mailString.Append("</font>");

        objmail.To.Add(emaild);
        if (emailCC != "")
        {
            objmail.CC.Add(emailCC);
        }
        objmail.IsBodyHtml = true;
        objmail.Body = Convert.ToString(mailString);
        objmail.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["Title"] + "<helpdesk@astix.in>");

        SmtpClient SmtpMail;
        SmtpMail = new SmtpClient();
        SmtpMail.Host = ConfigurationSettings.AppSettings["MailSMTPServer"];
        SmtpMail.Port = Convert.ToInt32(ConfigurationSettings.AppSettings["smtpPort"]);
        NetworkCredential loginInfo;
        loginInfo = new NetworkCredential();
        loginInfo.UserName = ConfigurationSettings.AppSettings["MailUsername"];
        loginInfo.Password = ConfigurationSettings.AppSettings["MailPassword"];
        SmtpMail.Credentials = loginInfo;
        SmtpMail.EnableSsl = true;
        SmtpMail.Timeout = 0;
        try
        {
            SmtpMail.Send(objmail);
            mailString = null;
        }
        catch (Exception ex)
        {
            FnWriteLogFile_Log("", ex.Message);
        }
    }
    public static void SendErrorMailMain(Exception ex, string FileName)
    {
        try
        {

            System.Text.StringBuilder mailString = new System.Text.StringBuilder();
            mailString.Append("<font  style='COLOR: #000080; FONT-FAMILY: Arial'  size=2>");
            mailString.Append("<p>Dear, </p>");
            mailString.Append("<p>Kindly check and correct below error </p></br>");
            mailString.Append("<p>Error Name : " + ex.Message.ToString() + "</p></br>");
            mailString.Append("<p>Error Description : " + ex.StackTrace.ToString() + "</p>");
            mailString.Append("<br><br><p>All the Best!</p>");
            mailString.Append("<br>Helpdesk");
            //mailString.Append("<p></p>");
            mailString.Append("</font>");


            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("astix@astixsolutions.com");
            mail.To.Add(System.Configuration.ConfigurationManager.AppSettings["ErrMailToSMS"].ToString());
            mail.Subject = "Error in File Uploading App " + FileName + " data upload) : " + DateTime.Now.ToString("dd-MMM-yyyy");



            mail.Body = mailString.ToString();
            mail.IsBodyHtml = true;

            //mail.Attachments.Add(new Attachment(fileName));
            SmtpServer.Port =Convert.ToInt32(ConfigurationSettings.AppSettings["smtpPort"]);
            SmtpServer.Credentials = new System.Net.NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["DMSUser"].ToString(), System.Configuration.ConfigurationManager.AppSettings["DMSGPassword"].ToString());
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
        }
        catch (Exception ex1) { }
    }
    public static void FnWriteLogFile_Log(string FileName, string message)
    {
        string LogPath_Log =HttpContext.Current.Server.MapPath("~/Logs/") + HttpContext.Current.Session["username"].ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd") + "_log.txt"; // This is Log File Path Where we Generate Log File
        
            using (var sw = new StreamWriter(LogPath_Log, true))
            {
                sw.WriteLine();
                sw.WriteLine("Date Time :" + DateTime.Now);
                sw.WriteLine(message);
            }
       
    }
}