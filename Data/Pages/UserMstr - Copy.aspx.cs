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
            hdnUserName.Value = Session["FullName"].ToString();
        }
    }

    [System.Web.Services.WebMethod()]
    public static string GetUserList(int LoginId)
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandText = "[spSniDbrGetUserList]";
        Scmd.CommandType = CommandType.StoredProcedure;
        Scmd.CommandTimeout = 0;
        //Scmd.Parameters.AddWithValue("@Login", LoginId);
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet Ds = new DataSet();
        Sdap.Fill(Ds);

        string[] SkipColumn = new string[2];
        SkipColumn[0] = "userid";
        SkipColumn[1] = "nodeid";

        return CreateHtmlTbl(Ds.Tables[0], SkipColumn, "User");
    }
    private static string CreateHtmlTbl(DataTable dt, string[] SkipColumn, string tblname)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table id='tbl" + tblname + "' class='table table-sm table-bordered mb-0 cls" + tblname + "'>");
        sb.Append("<thead>");
        sb.Append("<tr>");
        for (int j = 0; j < dt.Columns.Count; j++)
            if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToLower()))
                sb.Append("<th>" + dt.Columns[j].ColumnName + "</th>");

        sb.Append("<th>Action</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToLower()))
                    sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");

            sb.Append("<td><img src='../../Images/edit_blue.png' alt='edit' onclick='fnEdit(this)'/></td>");
            sb.Append("</tr>");
        }

        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }
}