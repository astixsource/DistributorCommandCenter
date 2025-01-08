using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Net.Mime;

public partial class frmSendEmailInvite : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["LoginID"] = 0;
        if (Session["LoginID"] == null)
        {
            Response.Redirect("~/Login.aspx");
        }
        else
        {
            if (!IsPostBack)
            {
                hdnLoginId.Value = Session["LoginID"].ToString();
            }
        }
    }

    [System.Web.Services.WebMethod()]
    public static string fngetdata(int CycleId)
    {
        SqlConnection con = null;
        con = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
        DataSet Ds = null;
        string stresponse = "";
        try
        {
            string storedProcName = "spGetSubdHawkeyeUserEmail";

            List<SqlParameter> sp = null;
            Ds = clsDbCommand.ExecuteQueryReturnDataSet(storedProcName, con, sp);


            StringBuilder str = new StringBuilder();
            StringBuilder str1 = new StringBuilder();

            if (Ds.Tables[0].Rows.Count > 0)
            {
                string[] SkipColumn = new string[15];

                SkipColumn[0] = "EmpNodeID";
                SkipColumn[1] = "FirstName";
                SkipColumn[2] = "SurName";
                SkipColumn[3] = "flgNewMail";
                SkipColumn[4] = "flgRescheduleMail";
                SkipColumn[5] = "BandID";
                SkipColumn[6] = "CalendarStartTime";
                SkipColumn[7] = "CalendarEndTime";
                SkipColumn[8] = "UserName";
                SkipColumn[9] = "UserPassword";
                SkipColumn[10] = "ParticipantCycleMappingID";
                SkipColumn[11] = "OrientationTime";
                SkipColumn[12] = "AssessmentStartDate";
                SkipColumn[13] = "AssessmentEndDate";
                SkipColumn[14] = "DCTypeID";

                int isSubmitted = 0;
                str.Append("<div id='dvtblbody' class='mb-3'><table id='tbldbrlist' class='table table-bordered table-sm mb-0' isSubmitted=" + isSubmitted + "><thead><tr>");

                string ss = "";

                str.Append("<th style='width:6%' >SrNo</th>");
                for (int j = 0; j < Ds.Tables[0].Columns.Count; j++)
                {
                    if (SkipColumn.Contains(Ds.Tables[0].Columns[j].ColumnName))
                        continue;

                    string sColumnName = Ds.Tables[0].Columns[j].ColumnName; ;

                    str.Append("<th " + ss + ">" + sColumnName + "</th>");
                }
                str.Append("<th><input type='checkbox' value='0' id='checkAll' onclick='check_uncheck_checkbox(this.checked)' > ALL</th>");
                str.Append("</tr></thead><tbody>");

                string OldParticipantId = "0";
                for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
                {
                    string DBRName = Ds.Tables[0].Rows[i]["DBRName"].ToString();
                    string Fname = Ds.Tables[0].Rows[i]["UserName"].ToString();
                    string Calenderstarttime = "";// Ds.Tables[0].Rows[i]["CalendarStartTime"].ToString();
                    string Calenderendtime = "";// Ds.Tables[0].Rows[i]["CalendarEndTime"].ToString();
                    string OrientationTime = "";// Ds.Tables[0].Rows[i]["OrientationTime"].ToString();


                    string subjectline = "";
                    string EmailID = Ds.Tables[0].Rows[i]["UserEmail"].ToString().Trim();
                    string UserName = Ds.Tables[0].Rows[i]["UserEmail"].ToString().Trim();
                    string Password = Ds.Tables[0].Rows[i]["UserEmail"].ToString().Trim();
                    string flgDisplayRow = "1";


                    str.Append("<tr MappingID = '0' DBRName='" + DBRName + "' EmailID = '" + EmailID + "'  EmpNodeID = '0' Fname ='" + Fname + "' UserName ='" + UserName + "' Password = '" + Password + "'> ");
                    str.Append("<td style='text-align:center'>" + (i + 1) + "</td>");
                    for (int j = 0; j < Ds.Tables[0].Columns.Count; j++)
                    {
                        string sColumnName = Ds.Tables[0].Columns[j].ColumnName;
                        if (SkipColumn.Contains(sColumnName))
                            continue;

                        str.Append("<td>" + Ds.Tables[0].Rows[i][j].ToString().Trim() + "</td>");
                    }
                    str.Append("<td><input type='checkbox' flg='1' value='1'></td>");

                    str.Append("</tr>");
                }
                str.Append("</tbody></table></div>");
            }
            else
                str.Append("");

            stresponse = str.ToString();
        }
        catch (Exception ex)
        {
            stresponse = "2|" + ex.Message;
        }
        finally
        {
            con.Dispose();
        }

        return stresponse;
    }

    [System.Web.Services.WebMethod()]
    public static string fnSave(object udt_DataSaving)
    {
        string strResponse = "";
        try
        {
            string strDataSaving = JsonConvert.SerializeObject(udt_DataSaving, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            DataTable dtDataSaving = JsonConvert.DeserializeObject<DataTable>(strDataSaving);
            dtDataSaving.TableName = "tblMeetingData";
            if (dtDataSaving.Rows[0][0].ToString() == "0")
            {
                dtDataSaving.Rows[0].Delete();
            }

            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            Scon.Open();
            foreach (DataRow drow in dtDataSaving.Rows)
            {
                try
                {
                    // DateTime dt1 = ;   //Convert.ToDateTime("2020-05-21 16:30");
                    string CalenderStartTime = "";    //Convert.ToDateTime(drow["CalenderStartTime"].ToString()).ToString("yyyy-MM-dd hh:mm:ss tt");
                    string CalenderEndTime = "";      //Convert.ToDateTime(drow["CalenderEndTime"].ToString()).ToString("yyyy-MM-dd hh:mm:ss tt");

                    string MailTo = drow["EmailId"].ToString();
                    string DBRName = drow["DBRName"].ToString();
                    string FName = drow["Fname"].ToString();
                    string UserName = drow["UserName"].ToString();
                    string Password = drow["Password"].ToString();

                    string strStatus = fnSendICSFIleToUsers(DBRName, FName, MailTo, UserName, Password);
                    drow["MailStatus"] = strStatus == "1" ? "Mail Sent" : strStatus;
                }
                catch (Exception ex)
                {
                    drow["MailStatus"] = "Error-" + ex.Message;
                }
            }

            strResponse = "0|" + JsonConvert.SerializeObject(dtDataSaving, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            Scon.Dispose();
        }
        catch (Exception ex)
        {
            strResponse = "1|" + ex.Message;
        }
        return strResponse;
    }

    public static string fnSendICSFIleToUsers(string DBRName, string FName, string MailTo, string UserName, string Password)
    {
        string strRespoonse = "1";
        try
        {

            string MailServer = ConfigurationManager.AppSettings["MailSMTPServer"].ToString();
            string MailUserName = ConfigurationManager.AppSettings["MailUsername"].ToString();
            string MailPwd = ConfigurationManager.AppSettings["MailPassword"].ToString();
            string flgActualUser = "1";// ConfigurationManager.AppSettings["flgActualUser"].ToString();
            string fromMail = ConfigurationManager.AppSettings["MailFrom"].ToString();

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("DF<help@astix.in>");
            msg.ReplyTo = new MailAddress("help@astix.in");
            msg.To.Add(MailTo);
            msg.Bcc.Add("dccsupportdesk@astix.in");
            msg.Subject = "Login Credentials";
            StringBuilder strBody = new StringBuilder();

            strBody.Append("<font  style='COLOR: #000080; FONT-FAMILY: Arial'  size=2>");
            strBody.Append("<p>Dear " + FName + ",</p>");
            strBody.Append("<p><b>Congratulations!!</b> Your account has been created in Distributor Command Center !! </p>");



            strBody.Append("<p>Please find below the link and your login credentials for accessing Distributor Command Center:</p>");
            strBody.Append("<p><b>Link: https://distributorcommandcenter.azurewebsites.net</b></p>");
            strBody.Append("<p><b>UserName: " + UserName + "</b></p>");
            strBody.Append("<p><b>Password: " + Password + "</b></p>");
            strBody.Append("<p>For visiting the Portal,Kindly <a href='https://distributorcommandcenter.azurewebsites.net'>Click here</a></p>");
            strBody.Append("<p><i>This account will give you access to reports for " + DBRName + "</i></p>");
            strBody.Append("<p><b>Thanks</b><br/><b>DCC Team</b></p>");
            strBody.Append("</font>");


            msg.IsBodyHtml = true;
            System.Net.Mime.ContentType HTMLType = new System.Net.Mime.ContentType("text/html");
            AlternateView avCal = AlternateView.CreateAlternateViewFromString(strBody.ToString(), HTMLType);

            msg.AlternateViews.Add(avCal);

            SmtpClient SmtpMail = new SmtpClient();
            SmtpMail.Host = MailServer;
            SmtpMail.Port = 587;
            string MUserName = MailUserName;
            string MPwd = MailPwd;
            NetworkCredential loginInfo = new NetworkCredential(MUserName, MPwd);
            SmtpMail.Credentials = loginInfo;
            SmtpMail.EnableSsl = true;
            SmtpMail.Timeout = int.MaxValue;

            SmtpMail.Send(msg);


        }
        catch (Exception ex)
        {
            strRespoonse = ex.Message;

            string sFunctioname = "EY Virtual DC – Login Credentials";
            string strSubject = "Error in EY Virtual DC – Login Credentials of " + FName;
            clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "Send Credential Page", sFunctioname, strSubject);
            strRespoonse = ex.Message;
        }
        return strRespoonse;
    }




}