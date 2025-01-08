using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;


public partial class ChannelLockingPeriodExt : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["LoginID"] == null || Session["username"] == null)
        {
            Response.Redirect("../../frmSessionTimeOut.aspx");
            return;
        }
        if (!IsPostBack)
        {
            hdnLoginId.Value = Session["LoginID"].ToString();
            hdnUserId.Value = Session["UserID"].ToString();
            hdnNodeID.Value = Session["NodeID"].ToString();
            hdnUserName.Value = Session["FullName"].ToString();
        }
    }

    [System.Web.Services.WebMethod()]
    public static string GetReport(string LoginID, string UserID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandTimeout = 0;
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandText = "[spGetMonths]";
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn = new string[3];
            SkipColumn[0] = "MonthVal";
            SkipColumn[1] = "YearVal";
            SkipColumn[2] = "flgSelect";

            return "0|^|" + CreateHtmlTbl(Ds.Tables[0], SkipColumn, "DateExt");
        }
        catch (Exception ex)
        {
            return "1|^|" + ex.Message;
        }
    }
    private static string CreateHtmlTbl(DataTable dt, string[] SkipColumn, string tblname)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table id='tbl" + tblname + "' class='table table-sm table-bordered mb-0 cls" + tblname + "'>");
        sb.Append("<thead>");
        sb.Append("<tr>");
        sb.Append("<th>#</th>");
        for (int j = 0; j < dt.Columns.Count; j++)
            if (!SkipColumn.Contains(dt.Columns[j].ColumnName))
                sb.Append("<th>" + dt.Columns[j].ColumnName + "</th>");

        sb.Append("<th>Action</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr flgEdit='0' MonthVal='" + dt.Rows[i]["MonthVal"].ToString() + "' YearVal='" + dt.Rows[i]["YearVal"].ToString() + "'>");
            sb.Append("<td>" + (i + 1).ToString() + "</td>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim()))
                {
                    if (dt.Columns[j].ColumnName.ToString().Trim() == "AllowedDate")
                        sb.Append("<td><input type='text' iden='lockingDate' class='cls-date' value='" + dt.Rows[i][j].ToString() + "' readonly='readonly'/></td>");
                    else
                        sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");
                }

            sb.Append("<td><a href='#' onclick='fnExtendDate(this);'>Extend</a></td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }


    [System.Web.Services.WebMethod()]
    public static string fnSave(string MonthVal, string YearVal, string strDate, string LoginID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spChanelChangeManageDate]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@MonthVal", MonthVal);
            Scmd.Parameters.AddWithValue("@YearVal", YearVal);
            Scmd.Parameters.AddWithValue("@Date", strDate);
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            return "0|^|";
        }
        catch (Exception ex)
        {
            return "1|^|" + ex.Message;
        }
    }
}