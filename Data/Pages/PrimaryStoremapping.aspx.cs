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


public partial class PrimaryStoremapping : System.Web.UI.Page
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
    public static string GetReport(string LoginID, string UserID, string NodeID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSubdHawkeyeGetPrimarySalesMapping]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@NodeID", NodeID);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn = new string[1];
            SkipColumn[0] = "BranchSubDNodeID";

            return "0|^|" + CreateHtmlTbl(Ds.Tables[0], SkipColumn, "Mapping") + "|^|" + CreateddlBranchMstr(Ds.Tables[1]);
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
            sb.Append("<tr flgEdit='0' strId='" + dt.Rows[i]["BranchSubDNodeID"].ToString() + "' PriBranchCode='" + (dt.Rows[i]["Primary Branch Code"].ToString() == "" ? "0" : dt.Rows[i]["Primary Branch Code"].ToString()) + "' LeapStoreCode='" + dt.Rows[i]["Leap Store Code"].ToString() + "'>");
            sb.Append("<td>" + (i + 1).ToString() + "</td>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim()))
                {
                    if (dt.Columns[j].ColumnName.ToString().Trim() == "Primary Branch Code")
                        sb.Append("<td><select iden='branchcode' onchange='fnSelectBranch(this);' disabled></select></td>");
                    else if (dt.Columns[j].ColumnName.ToString().Trim() == "Primary Branch Name")
                        sb.Append("<td><select iden='branchname' onchange='fnSelectBranch(this);' disabled></select></td>");
                    else if (dt.Columns[j].ColumnName.ToString().Trim() == "Leap Store Code")
                        sb.Append("<td><input iden='leapstore' type='text' value='" + dt.Rows[i][j].ToString() + "' disabled /></td>");
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
    private static string CreateddlBranchMstr(DataTable dt)
    {
        StringBuilder sbCode = new StringBuilder();
        StringBuilder sbName = new StringBuilder();

        sbCode.Append("<option value='0'>-- Select --</option>");
        sbName.Append("<option value='0'>-- Select --</option>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sbCode.Append("<option value='" + dt.Rows[i]["BranchCode"].ToString() + "'>" + dt.Rows[i]["BranchCode"].ToString() + "</option>");
            sbName.Append("<option value='" + dt.Rows[i]["BranchCode"].ToString() + "'>" + dt.Rows[i]["BranchName"].ToString() + "</option>");
        }

        return sbCode.ToString() + "|^|" + sbName.ToString();
    }


    [System.Web.Services.WebMethod()]
    public static string fnSave(string LoginID, string UserID, string NodeID, string BranchCode, string BranchName, string StoreCode)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSubdHawkeyeManagePrimarySalesMapping]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@NodeID", NodeID);
            Scmd.Parameters.AddWithValue("@strBranchCodeMapping", BranchCode);
            Scmd.Parameters.AddWithValue("@strBranchNameMapping", BranchName);
            Scmd.Parameters.AddWithValue("@strSubDMapping", StoreCode);
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