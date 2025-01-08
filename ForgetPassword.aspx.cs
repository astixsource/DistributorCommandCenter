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
        hdnFlagType.Value = "91";
        lblTittle.InnerHtml = "Forget Password";

        //hdnFlagType.Value = "92";
        //lblTittle.InnerHtml = "Set Your Password";
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "spForgotPWD_GetLink";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@EmailID", txtEmail.Text);
            Scmd.CommandTimeout = 0;
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);
            Scmd.Dispose();
            Sdap.Dispose();

            if (Ds.Tables[0].Rows[0][0].ToString() != "-1")
            {
                string[] Param = new string[2];
                Param[0] = Ds.Tables[0].Rows[0]["strKey"].ToString();
                Param[1] = Ds.Tables[0].Rows[0]["ValidTill"].ToString();

                Mail.MailBody(hdnFlagType.Value, Ds.Tables[0].Rows[0]["UserName"].ToString(), Param, txtEmail.Text, "", "");

                txtEmail.Text = "";
                dvMessage.InnerHtml = "Password reset link shared on your Registered Email-ID !";
            }
            else
                dvMessage.InnerHtml = "Provided Email-ID is incorrect or not registered !";
        }
        catch(Exception ex)
        {
            dvMessage.InnerHtml = "Error : " + ex.Message;
        }
    }
}