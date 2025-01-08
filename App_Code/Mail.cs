using System;
using System.IO;
using System.Web;
using System.Text;
using System.Data;
using System.Net.Mail;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mail;
using Azure;
using Azure.Communication.Email;

/// <summary>
/// Summary description for Mail
/// </summary>
public static class Mail
{
    public static string MailBody(string FlgType, string ToName, string[] Param, string ToMail, string BCCMail, string CCMail)
    {
        int flgValidate = 0;
        string Sub = "", AttachFile = "", FromMail = "";
        StringBuilder sbMsg = new StringBuilder();

        switch (FlgType)
        {
            case "91":                   // Forget Password Reset
                sbMsg.Clear();
                sbMsg.Append("Dear <b>" + ToName + ",</b>");
                sbMsg.Append("<br/><br/>We've received a request to reset the Password for your account.");
                sbMsg.Append("<br/><br/>You can reset the password by clicking the button below :");
                sbMsg.Append("<br/><br/><a style='color: #fff; background: #0080c0; border: 1px solid #0080c0; border-radius: 5px; padding: 5px 18px; margin: 2px; text-decoration: none;' href='" + ConfigurationManager.AppSettings["PortalUrl"].ToString() + "PasswordReset.aspx?p=" + FlgType + Param[0] + "'>Reset Your Password</a>.");
                sbMsg.Append("<br/><br/>This link will be Active till " + Param[1] + ".");
                sbMsg.Append("<br/>If your link has expired, you can always <a style='color: #6c78e6; text-decoration: underline;' href='" + ConfigurationManager.AppSettings["PortalUrl"].ToString() + "ForgetPassword.aspx'>request another</a>");
                sbMsg.Append("<br/><br/>If you didn't request a password reset, you can safely ignore this Email.");
                sbMsg.Append("<br/><br/><b>Regards</b>");
                sbMsg.Append("<br/>Distributor Command Center");

                flgValidate = 1;
                Sub = "Distributor Command Center : Reset your Password ";
                FromMail = ConfigurationManager.AppSettings["MailFrom"].ToString();
                break;
            case "92":                   // Welcome Mail after New Registration
                sbMsg.Clear();
                sbMsg.Append("Congratulation <b>" + ToName + ",</b>");
                sbMsg.Append("<p style='color: #0e2e6d; font-size: 20px; font-weight: 700; text-transform: uppercase; font-style: italic;'>Welcome to the Family !!</p>");
                sbMsg.Append("We are pleased to inform you that your account has been created.");
                //sbMsg.Append("<br/><br/>Your <b>Registered Email-ID</b>, provided while registration, will be your <b>Username</b>.");
                sbMsg.Append("<br/>You can set your password by clicking the button below :");
                sbMsg.Append("<br/><br/><a style='color: #fff; background: #0080c0; border: 1px solid #0080c0; border-radius: 5px; padding: 5px 18px; margin: 2px; text-decoration: none;' href='" + ConfigurationManager.AppSettings["PortalUrl"].ToString() + "PasswordReset.aspx?p=0" + FlgType + Param[0] + "'>Set Your Password</a>.");
                sbMsg.Append("<br/><br/>This link will be Active till " + Param[1] + ".");
                sbMsg.Append("<br/>If your link has expired, you can always <a style='color: #6c78e6; text-decoration: underline;' href='" + ConfigurationManager.AppSettings["PortalUrl"].ToString() + "ForgetPassword.aspx?p=" + FlgType + Param[0] + "'>request another</a>");
                sbMsg.Append("<br/><br/><b>Cheers</b>");
                sbMsg.Append("<br/>Distributor Command Center");

                flgValidate = 1;
                Sub = "Distributor Command Center : Your Account has been Approved ";
                FromMail = ConfigurationManager.AppSettings["MailFrom"].ToString();
                break;
        }

        //if (flgValidate > 0)
        //    return SendMail(ToMail, BCCMail, CCMail, FromMail, Sub, sbMsg.ToString(), AttachFile, FlgType);
        //else
            return "1";
    }
    //public static string SendMail(string ToMail, string BCCMail, string CCMail, string FromMail, string sub, string msg, string AttachFile, string FlgType)
    //{
    //    try
    //    {
    //        MailMessage mail = new MailMessage();
    //        mail.From = new MailAddress(FromMail);
    //        if (ConfigurationManager.AppSettings["isTesting"].ToString() != "1")
    //        {
    //            if (ToMail != "")
    //                mail.To.Add(ToMail);
    //            if (BCCMail != "")
    //                mail.Bcc.Add(BCCMail);
    //            if (CCMail != "")
    //                mail.CC.Add(CCMail);
    //        }
    //        else
    //        {
    //            mail.To.Add(ConfigurationManager.AppSettings["TestMailIds"].ToString());
    //        }
    //        mail.Subject = sub;
    //        mail.Body = msg;
    //        mail.IsBodyHtml = true;
    //        if (AttachFile != "")
    //            mail.Attachments.Add(new Attachment(AttachFile));

    //        SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["MailSMTPServer"].ToString());
    //        SmtpServer.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailUsername"].ToString(), ConfigurationManager.AppSettings["MailPassword"].ToString());
    //        SmtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"].ToString());
    //        SmtpServer.EnableSsl = false;
    //        SmtpServer.Send(mail);

    //        Log.CreateLog("MailLog", FlgType, "Success", ToMail, msg);
    //        return "0";
    //    }
    //    catch (Exception ex)
    //    {
    //        Log.CreateLog("MailLog", FlgType, "Failed", ToMail, msg);
    //        return "1^" + ex.Message;
    //    }
    //}



    public static string SendMail(string ToMail, string BCCMail, string CCMail, string sub, string msg, string FileName)
    {
        var connectionString = "endpoint=https://astixemailcommunication.india.communication.azure.com/;accesskey=eY/ca2ZawDDXmJx1KvbW0FXw5CbMmucrsW+mjBqE9urodCYTNJeiBeRq3vjX/s7cVlCymgjphLEPbeF9IJRSuw==";
        var emailClient = new EmailClient(connectionString);

        var emailRecipients = new EmailRecipients();
        if (ConfigurationSettings.AppSettings["flgTesting"].ToString() == "0")
        {
            if (ToMail != "")
            {
                for (int i = 0; i < ToMail.Split(',').Length; i++)
                {
                    emailRecipients.To.Add(new EmailAddress(ToMail.Split(',')[i].Trim()));
                }
            }

            if (BCCMail != "")
            {
                for (int i = 0; i < BCCMail.Split(',').Length; i++)
                {
                    emailRecipients.BCC.Add(new EmailAddress(BCCMail.Split(',')[i].Trim()));
                }
            }

            if (CCMail != "")
            {
                for (int i = 0; i < CCMail.Split(',').Length; i++)
                {
                    emailRecipients.CC.Add(new EmailAddress(CCMail.Split(',')[i].Trim()));
                }
            }


            //CreateLogfile(DateTime.Now.ToString("hh:mm:ss tt") + "  :  Mail configured to send to To - " + ToMail + " , BCC - " + BCCMail + " , CC - " + CCMail + " (Live User)");
        }
        else
        {
            ToMail = ConfigurationSettings.AppSettings["TestingMailId"].ToString();
            if (ToMail != "")
            {
                for (int i = 0; i < ToMail.Split(',').Length; i++)
                {
                    emailRecipients.To.Add(new EmailAddress(ToMail.Split(',')[i].Trim()));
                }
            }

            //CreateLogfile(DateTime.Now.ToString("hh:mm:ss tt") + "  :  Mail configured to send to - " + ToMail + " (Test User)");
        }

        var emailContent = new EmailContent(sub) { PlainText = null, Html = msg };
        var emailMessage = new EmailMessage(
            senderAddress: ConfigurationSettings.AppSettings["MailSender"],      //The email address of the domain registered with the Communication Services resource
            recipients: emailRecipients,
            content: emailContent);

        if (FileName != "")
        {
            var contentType = "";
            var content = new BinaryData(System.IO.File.ReadAllBytes(FileName));
            var attachmentName = "Attachment_" + DateTime.Now.ToString("dd-MMM-yyyy") + ".xlsx";
            var emailAttachment = new EmailAttachment(attachmentName, contentType, content);
            emailMessage.Attachments.Add(emailAttachment);
        }

        var emailSendOperation = emailClient.Send(wait: WaitUntil.Completed, message: emailMessage);
        //CreateLogfile(DateTime.Now.ToString("hh:mm:ss tt") + "  :  Mail Status - " + emailSendOperation.Value.Status);
        return "0";
    }
    public static void SendErrorMail(string sub, string msg)
    {
        var connectionString = "endpoint=https://astixemailcommunication.india.communication.azure.com/;accesskey=eY/ca2ZawDDXmJx1KvbW0FXw5CbMmucrsW+mjBqE9urodCYTNJeiBeRq3vjX/s7cVlCymgjphLEPbeF9IJRSuw==";
        var emailClient = new EmailClient(connectionString);

        var emailRecipients = new EmailRecipients();
        string ToMail = ConfigurationManager.AppSettings["ErrorMailTo"].ToString();
        if (ToMail != "")
        {
            for (int i = 0; i < ToMail.Split(',').Length; i++)
            {
                emailRecipients.To.Add(new EmailAddress(ToMail.Split(',')[i].Trim()));
            }
        }

        //CreateLogfile(DateTime.Now.ToString("hh:mm:ss tt") + "  :  Error Mail configured to send to - " + ToMail);

        var emailContent = new EmailContent(sub) { PlainText = null, Html = msg };
        var emailMessage = new EmailMessage(
            senderAddress: ConfigurationSettings.AppSettings["MailSender"],      //The email address of the domain registered with the Communication Services resource
            recipients: emailRecipients,
            content: emailContent);

        var emailSendOperation = emailClient.Send(wait: WaitUntil.Completed, message: emailMessage);
        //CreateLogfile(DateTime.Now.ToString("hh:mm:ss tt") + "  :  Error Mail Status - " + emailSendOperation.Value.Status);
    }
}