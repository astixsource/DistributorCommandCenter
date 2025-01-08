using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;


public partial class frmSessionTimeOut : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) {
            string flg =  Request.QueryString["flgcallfrom"] == null ? "0" : Request.QueryString["flgcallfrom"];
            string smsg = "";
            if (flg == "0") {
                smsg = "Oops, your session has expired!";
            }
            else if (flg == "3") {
                smsg = "Please check if you are already logged-in at another tab/browser/device. In that case, please logout from your running session before you try to relogin.</br>In case you had lost internet connectivity, please wait for 2 minutes before you can relogin.";
            }
            else if (flg == "4")
            {
                smsg = "You have been successfully logged out!";
            }
            else if (flg == "5")
            {
                smsg = "Something went wrong, kindly try again.";
            }
            else if (flg == "6")
            {
                smsg = "User is not registered in Marico Map DB,kindly contact site admin.";
            }
            else {
                smsg = "Wrong username & password";
            }

            divMsgContainer.InnerHtml = smsg;
            Session.Abandon();
            Session.RemoveAll();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
       }
    }

   

    protected void btnCGSubmit_Click(object sender, EventArgs e)
    {
        Response.Redirect("~/frmLogin.aspx");
    }
}