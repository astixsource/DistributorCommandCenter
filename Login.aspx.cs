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
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Owin;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string flgCallingfrom = Request.QueryString["flgcall"] == null ? "0" : Request.QueryString["flgcall"].ToString();
        if (flgCallingfrom == "0")
        {
            if (Request.IsAuthenticated == false)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "https://maricomap.azurewebsites.net/Login.aspx" },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return;
            }
        }
        else
        {
            string pgname = Request.QueryString["pgname"] == null ? "" : "?flgcall=1&pgname=" + Request.QueryString["pgname"].ToString();
            Session["Pagetobeopen"] = Request.QueryString["pgname"] == null ? "" : Request.QueryString["pgname"].ToString(); ;
            if (Request.IsAuthenticated == false)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "https://maricomap.azurewebsites.net/Login.aspx" + pgname },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
                return;
            }
        }
        if (!IsPostBack)
        {
            // fnSignIn("mukhtyar@astix.in");
            clsADUserInfo clsObj = new clsADUserInfo();
            var UserInfo = clsObj.fnGetUserDetail();
            if (UserInfo.Result.Mail != null)
            {
                if (UserInfo.Result.Mail != "")
                {
                    fnSignIn(UserInfo.Result.Mail);
                }
                else
                {
                    Response.Redirect("~/frmSessionTimeOut.aspx?flgcallfrom=5");
                }
            }
            else
            {
                Response.Redirect("~/frmSessionTimeOut.aspx?flgcallfrom=5");
            }
        }
    }

    public void fnSignIn(string UserName)
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);

        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandText = "spSecUserLogin";
        Scmd.CommandType = CommandType.StoredProcedure;
        Scmd.Parameters.AddWithValue("@UserName", UserName);
        Scmd.Parameters.AddWithValue("@UserPwd", UserName);
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
            Session["username"] = UserName;
            Session["FullName"] = Ds.Tables[0].Rows[0]["UserFullName"].ToString();
            Session["loginPwd"] = UserName;
            Session["LoginID"] = Ds.Tables[0].Rows[0]["LoginID"].ToString();
            Session["RoleID"] = Ds.Tables[0].Rows[0]["RoleId"].ToString();
            Session["UserID"] = Ds.Tables[0].Rows[0]["UserID"].ToString();
            Session["NodeID"] = Ds.Tables[0].Rows[0]["NodeID"].ToString();
            if (Convert.ToString(Session["Pagetobeopen"]) != "") {
                string pgname1 =Convert.ToString(Session["Pagetobeopen"]);
                Session["Pagetobeopen"] = null;
                Response.Redirect("~/"+ pgname1);
            }
            else
            {
                if (Ds.Tables[0].Rows[0]["RoleId"].ToString() == "1")
                {
                    Response.Redirect("~/Data/Pages/frmItemlistforvalidation.aspx?typeid=1");
                }
                else if (Ds.Tables[0].Rows[0]["RoleId"].ToString() == "4" || Ds.Tables[0].Rows[0]["RoleId"].ToString() == "5")
                {
                    Response.Redirect("~/Data/Pages/AdminDashboard.aspx");
                }
                else
                {
                    Response.Redirect("~/Data/Pages/Home.aspx");
                }
            }
        }
        else
        {
            //txtLoginID.Text = "";
            //txtPassword.Text = "";
            Response.Redirect("~/frmSessionTimeOut.aspx?flgcallfrom=6");
        }
        Ds.Dispose();


    }
}