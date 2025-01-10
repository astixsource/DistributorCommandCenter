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
        lblTittle.InnerHtml = "";
        if (Request.QueryString.AllKeys.Contains("p"))
        {
            string p = Request.QueryString["p"];
            if (p.Substring(0, 2) == "91")
                lblTittle.InnerHtml = "Reset Password";
            else
                lblTittle.InnerHtml = "Set Your Password";

            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "spForgotPWD_ValidateLink";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@Key", p.Substring(2));
            Scmd.CommandTimeout = 0;
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);
            Scmd.Dispose();
            Sdap.Dispose();

            hdnCode.Value = Ds.Tables[0].Rows[0]["UserID"].ToString();
        }
        else
            Response.Redirect("frmLogin.aspx");
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "spForgotPWD_ChangePassword";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@UserID", hdnCode.Value);
            Scmd.Parameters.AddWithValue("@Password", HttpUtility.HtmlEncode(txtNewPassword.Text));
            Scmd.CommandTimeout = 0;
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);
            Scmd.Dispose();
            Sdap.Dispose();

            txtNewPassword.Text = "";
            txtRePassword.Text = "";
            dvMessage.InnerHtml = "Password reset successfully. Please re-login in your account, using new password !";
        }
        catch(Exception ex)
        {
            dvMessage.InnerHtml = "Error : " + ex.Message;
        }
    }
}