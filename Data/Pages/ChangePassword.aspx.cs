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


public partial class Home : System.Web.UI.Page
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

            hdnIsFirst.Value = Request.QueryString["isf"];
        }
    }


    [System.Web.Services.WebMethod()]
    public static string fnSubmit(string UserID, string NewPassword)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "spForgotPWD_ChangePassword";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@Password", NewPassword);
            Scmd.CommandTimeout = 0;
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);
            Scmd.Dispose();
            Sdap.Dispose();

            return "0|^|";
        }
        catch (Exception ex)
        {
            return "1|^|" + ex.Message;
        }
    }
}