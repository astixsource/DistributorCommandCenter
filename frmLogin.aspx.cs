using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Xml;
using System.IO;

public partial class frmLogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string flgCallingfrom = Request.QueryString["flgcall"] == null ? "0" : Request.QueryString["flgcall"].ToString();
        //if (flgCallingfrom == "1")
        //{
        //    Session["Pagetobeopen"] = Request.QueryString["pgname"] == null ? "" : Request.QueryString["pgname"].ToString();
        //    if (Session["LoginID"] != null)
        //    {
        //        Response.Redirect("~/" + Convert.ToString(Session["Pagetobeopen"]));
        //    }
        //}
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);

        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandText = "spSecUserLogin";
        Scmd.CommandType = CommandType.StoredProcedure;
        Scmd.Parameters.AddWithValue("@UserName", txtLoginID.Text);
        Scmd.Parameters.AddWithValue("@UserPwd", txtPassword.Text);
        Scmd.Parameters.AddWithValue("@SessionIdNw", Session.SessionID);
        Scmd.Parameters.AddWithValue("@IPAddress", Request.ServerVariables["REMOTE_ADDR"]);
        Scmd.Parameters.AddWithValue("@BrwsrVer", Request.Browser.Type);
        Scmd.Parameters.AddWithValue("@ScrRsltn", hdnResolution.Value);
        Scmd.CommandTimeout = 0;
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet Ds = new DataSet();
        Sdap.Fill(Ds);
        Scmd.Dispose();
        Sdap.Dispose();

        if (Ds.Tables[0].Rows[0]["LoginResult"].ToString() == "2" || Ds.Tables[0].Rows[0]["LoginResult"].ToString() == "3")
        {
            Session["username"] = txtLoginID.Text;
            Session["FullName"] = Ds.Tables[0].Rows[0]["UserFullName"].ToString();
            Session["loginPwd"] = txtPassword.Text;
            Session["LoginID"] = Ds.Tables[0].Rows[0]["LoginID"].ToString();
            Session["RoleID"] = Ds.Tables[0].Rows[0]["RoleId"].ToString();
            Session["UserID"] = Ds.Tables[0].Rows[0]["UserID"].ToString();
            Session["NodeID"] = Ds.Tables[0].Rows[0]["NodeID"].ToString();
            Session["SiteName"] = Ds.Tables[0].Rows[0]["SiteName"].ToString();
            dvMessage.InnerHtml = "";

            if (Ds.Tables[0].Rows[0]["flgPasswordChange"].ToString() == "1")
                Response.Redirect("~/Data/Pages/ChangePassword.aspx?isf=1");
            else
                Response.Redirect("~/Data/Pages/Home.aspx");
        }
        else
        {
            txtLoginID.Text = "";
            txtPassword.Text = "";
            dvMessage.InnerHtml = "Incorrect username or password.";
        }
        Ds.Dispose();


    }
}