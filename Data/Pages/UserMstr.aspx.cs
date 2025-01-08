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


public partial class UserMstr : System.Web.UI.Page
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

            GetBranchHier();
        }
    }
    private void GetBranchHier()
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandText = "[spSniDbrGetLocationHierarchy]";
        Scmd.CommandType = CommandType.StoredProcedure;
        Scmd.CommandTimeout = 0;
        Scmd.Parameters.AddWithValue("@UserID", hdnUserId.Value);
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet Ds = new DataSet();
        Sdap.Fill(Ds);

        StringBuilder sbHier = new StringBuilder();
        DataRow[] drTemp = Ds.Tables[0].Select("PID=0 And PlvlNodeType=0");
        if (drTemp.Length > 0)
        {
            sbHier.Append("<ul class='mt-2 ml-2 p-0'>");
            DataTable dttemp = drTemp.CopyToDataTable();
            for (int i = 0; i < dttemp.Rows.Count; i++)
            {
                sbHier.Append("<li id='" + dttemp.Rows[i]["id"] + "' ntype='" + dttemp.Rows[i]["lvlNodeType"] + "' pid='" + dttemp.Rows[i]["PID"] + "' pntype='" + dttemp.Rows[i]["PlvlNodeType"] + "'>");

                DataRow[] drSubTemp = Ds.Tables[0].Select("PID='" + dttemp.Rows[i]["id"].ToString() + "' And PlvlNodeType='" + dttemp.Rows[i]["lvlNodeType"].ToString() + "'");
                if (drSubTemp.Length > 0)
                {
                    sbHier.Append("<img title='Expand' src='../../Images/icoAdd.gif' onclick='fnExpand(this);'/>");
                    sbHier.Append("<input type='checkbox' class='ml-1 mr-1' IslastNode='0' ntype='" + dttemp.Rows[i]["lvlNodeType"] + "' code='" + dttemp.Rows[i]["lvlCode"] + "' onclick='fnCheckNode(this);'>");
                    sbHier.Append("<span>" + dttemp.Rows[i]["LvlName"] + "</span>");
                    //sbHier.Append("<span onclick='fnNodeSelection(this);'>" + dttemp.Rows[i]["LvlName"] + "</span>");
                }
                else
                {
                    sbHier.Append("<img src='../../Images/trans.png' />");
                    sbHier.Append("<input type='checkbox' class='ml-1 mr-1' IslastNode='1' ntype='" + dttemp.Rows[i]["lvlNodeType"] + "' code='" + dttemp.Rows[i]["lvlCode"] + "' onclick='fnCheckNode(this);'>");
                    sbHier.Append("<span>" + dttemp.Rows[i]["LvlName"] + "</span>");
                    //sbHier.Append("<span onclick='fnNodeSelection(this);'>" + dttemp.Rows[i]["LvlName"] + "</span>");
                }

                if (drSubTemp.Length > 0)
                    sbHier.Append(GetSubHier(Ds.Tables[0], drSubTemp.CopyToDataTable()));

                sbHier.Append("</li>");
            }
            sbHier.Append("</ul>");
        }
        hdnBranchHierMstr.Value = sbHier.ToString();
    }
    private static string GetSubHier(DataTable dtMstr, DataTable dttemp)
    {
        StringBuilder sbHier = new StringBuilder();
        sbHier.Append("<ul style='display : none;'>");
        for (int i = 0; i < dttemp.Rows.Count; i++)
        {
            sbHier.Append("<li id='" + dttemp.Rows[i]["id"] + "' ntype='" + dttemp.Rows[i]["lvlNodeType"] + "' pid='" + dttemp.Rows[i]["PID"] + "' pntype='" + dttemp.Rows[i]["PlvlNodeType"] + "'>");

            DataRow[] drSubTemp = dtMstr.Select("PID='" + dttemp.Rows[i]["id"].ToString() + "' And PlvlNodeType='" + dttemp.Rows[i]["lvlNodeType"].ToString() + "'");
            if (drSubTemp.Length > 0)
            {
                sbHier.Append("<img title='Expand' src='../../Images/icoAdd.gif' onclick='fnExpand(this);'/>");
                sbHier.Append("<input type='checkbox' class='ml-1 mr-1' IslastNode='0' ntype='" + dttemp.Rows[i]["lvlNodeType"] + "' code='" + dttemp.Rows[i]["lvlCode"] + "' onclick='fnCheckNode(this);'>");
                sbHier.Append("<span>" + dttemp.Rows[i]["LvlName"] + "</span>");
                //sbHier.Append("<span onclick='fnNodeSelection(this);'>" + dttemp.Rows[i]["LvlName"] + "</span>");
            }
            else
            {
                sbHier.Append("<img src='../../Images/trans.png' />");
                sbHier.Append("<input type='checkbox' class='ml-1 mr-1' IslastNode='1' ntype='" + dttemp.Rows[i]["lvlNodeType"] + "' code='" + dttemp.Rows[i]["lvlCode"] + "' onclick='fnCheckNode(this);'>");
                sbHier.Append("<span>" + dttemp.Rows[i]["LvlName"] + "</span>");
                //sbHier.Append("<span onclick='fnNodeSelection(this);'>" + dttemp.Rows[i]["LvlName"] + "</span>");
            }

            if (drSubTemp.Length > 0)
                sbHier.Append(GetSubHier(dtMstr, drSubTemp.CopyToDataTable()));

            sbHier.Append("</li>");
        }
        sbHier.Append("</ul>");
        return sbHier.ToString();
    }


    [System.Web.Services.WebMethod()]
    public static string GetReport(string LoginID, string UserID, string NodeID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSniDbrGetUserList]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            //Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn = new string[3];
            SkipColumn[0] = "UserID";
            SkipColumn[1] = "NodeID";
            SkipColumn[2] = "strAccess";

            return "0|^|" + CreateHtmlTbl(Ds.Tables[0], SkipColumn, "User");
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
            sb.Append("<tr flgEdit='0' UserID='" + dt.Rows[i]["UserID"].ToString() + "' UserName='" + dt.Rows[i]["UserName"].ToString() + "' UserEmailID='" + dt.Rows[i]["UserEmailID"].ToString() + "' Active='" + dt.Rows[i]["Active"].ToString() + "' Branches='" + dt.Rows[i]["strAccess"].ToString() + "' BranchCount='" + dt.Rows[i]["Branches"].ToString() + "'>");
            sb.Append("<td>" + (i + 1).ToString() + "</td>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim()))
                {
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
    public static string fnSave(string UserID, string UserName, string UserEmailID, string flgActive, string strBranch)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSniDbrManageUsers]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@UserName", UserName);
            Scmd.Parameters.AddWithValue("@UserEmailID", UserEmailID);
            Scmd.Parameters.AddWithValue("@flgActive", flgActive);
            Scmd.Parameters.AddWithValue("@strBranch", strBranch);
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