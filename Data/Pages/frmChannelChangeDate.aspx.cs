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
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using ListItem = System.Web.UI.WebControls.ListItem;

public partial class frmChannelChangeDate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Session["LoginID"] = "1";
        //Session["username"] = "1";
        //Session["UserID"] = "1";
        //Session["NodeID"] = "1";
        //Session["FullName"] = "1";
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
            MonthYearDDL(hdnUserId.Value, hdnLoginId.Value);
        }
    }
    private void MonthYearDDL(string UserID, string LoginID)
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandText = "[spGetMonths]";
        Scmd.Parameters.AddWithValue("@UserID", UserID);
        Scmd.Parameters.AddWithValue("@LoginID", LoginID);
        Scmd.CommandType = CommandType.StoredProcedure;
        Scmd.CommandTimeout = 0;
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet Ds = new DataSet();
        Sdap.Fill(Ds);
        ddlMonthYear.Items.Add(new ListItem("--Select--", "0"));
        for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
        {
            ddlMonthYear.Items.Add(new ListItem(Ds.Tables[0].Rows[i]["MOnthYear"].ToString(), Ds.Tables[0].Rows[i]["MonthVal"].ToString() + "-" + Ds.Tables[0].Rows[i]["YearVal"].ToString() + "-" + Convert.ToString(Ds.Tables[0].Rows[i]["StartDate"]) + "-" + Convert.ToString(Ds.Tables[0].Rows[i]["EndDate"])));
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
            sb.Append("<tr flgEdit='0' strId='" + dt.Rows[i]["BranchSubDNodeID"].ToString() + "' IsJBPDone='" + dt.Rows[i]["JBP Done"].ToString() + "' Img='" + dt.Rows[i]["ImageName"].ToString() + "' IsInTemp='0' NewImg=''>");
            sb.Append("<td>" + (i + 1).ToString() + "</td>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim()))
                {
                    if (dt.Columns[j].ColumnName.ToString().Trim() == "JBP Done")
                        sb.Append("<td><select disabled><option value='0'>No</option><option value='1'>Yes</option></select></td>");
                    else if (dt.Columns[j].ColumnName.ToString().Trim() == "ImageName")
                        sb.Append("<td><img src='../../Images/Img-icon.png' alt='edit' onclick='fnImg(this)'/></td>");
                    else
                        sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");
                }

            sb.Append("<td><img src='../../Images/edit_blue.png' alt='edit' onclick='fnEdit(this)'/></td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }


    [System.Web.Services.WebMethod()]
    public static string fnSave(string MonthVal, string YearVal, string StartDate, string EndDate, string LoginID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spChanelChangeManageDate]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@MonthVal", MonthVal);
            Scmd.Parameters.AddWithValue("@YearVal", YearVal);
            Scmd.Parameters.AddWithValue("@StartDate", StartDate);
            Scmd.Parameters.AddWithValue("@EndDate", EndDate);
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