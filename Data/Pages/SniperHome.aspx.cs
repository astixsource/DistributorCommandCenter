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

            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandTimeout = 0;
            Scmd.CommandText = "[spSecMenuButton_Sinper]";
            Scmd.Parameters.AddWithValue("@UserID", hdnUserId.Value);
            Scmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
            {
                //if(Ds.Tables[0].Rows[i]["flgApplicable"].ToString() == "1")
                //    sb.Append("<input type='button' class='btns btn-submit m-3 mb-0' Menuflg='" + Ds.Tables[0].Rows[i]["mnID"].ToString() + "' value='" + Ds.Tables[0].Rows[i]["MenuDescription"].ToString() + "' onclick='fnMenu(this);' />");

                if (Ds.Tables[0].Rows[i]["flgApplicable"].ToString() == "1")
                    sb.Append("<input type='button' class='btns btn-submit m-3 mb-0' Menuflg='" + Ds.Tables[0].Rows[i]["mnID"].ToString() + "' value='" + Ds.Tables[0].Rows[i]["MenuDescription"].ToString() + "' onclick='fnAction(" + Ds.Tables[0].Rows[i]["mnID"].ToString() + ");' />");
            }
            divbtns.InnerHtml = sb.ToString();
        }
    }    
}