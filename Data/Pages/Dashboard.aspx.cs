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


public partial class Data_Dashboard_Dashboard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(Session["LoginID"] == null || Session["username"]==null || Session["SiteName"]==null)
        {
            Response.Redirect("../../frmSessionTimeOut.aspx");
            return;
        }
        if (!IsPostBack)
        {
            hdnLoginId.Value = Session["LoginID"].ToString();
            hdnUserId.Value = Session["UserID"].ToString();
            hdnUserName.Value = Session["FullName"].ToString();
            hdnSiteName.Value = Session["SiteName"].ToString();
            string mnuId = Request.QueryString["id"] == null ? "0" : Request.QueryString["id"].ToString();
            bool IsRslapplied = true;
            switch (mnuId)
            {
                case "9":
                    hdnReportId.Value = "95668075-5f22-41c2-a7ee-961614f3a510";
                    break;
                case "10":
                    hdnReportId.Value = "879ece0f-52b1-4ad4-a29d-706ecd24d562";
                    break;
                case "11":
                    hdnReportId.Value = "e81a3293-278b-472a-8545-f7a722df6571";
                    break;
                case "12":
                    hdnReportId.Value = "2e849ed7-fe3f-4877-8a0c-6b3fbce81261";
                    //hdnReportId.Value = "84c42a4f-e08f-446a-9e62-b5d14454f5fb";
                    break;
                case "14":
                    hdnReportId.Value = "3f9c3aa2-38d6-4c53-a627-8bec9662b9a9";
                    break;
                case "15":
                    hdnReportId.Value = "88529f3a-e785-4bce-a75c-946c2c486add";
                    break;
                case "16":
                    hdnReportId.Value = "8b74090d-f7fa-4bb4-b063-ab3cdfd91844";
                    break;
                case "17":
                    hdnReportId.Value = "d0159ab8-1859-4ac5-9c5b-f58563f1ef9d";
                    break;
                case "18":
                    hdnReportId.Value = "9de5b048-f3c8-4f3c-9429-9f60a7d623e5";
                    break;
                case "20":
                    hdnReportId.Value = "2edf7c25-ad2c-4de4-8d03-1b687dc2bf90";
                    break;
                case "21":
                    hdnReportId.Value = "670b89f7-7157-4cfb-9027-7b019507c501";
                    break;
                case "28":
                    IsRslapplied = false;
                    hdnReportId.Value = "ca01a50f-492a-4a13-af2a-3a2a88321c89";
                    break;
                case "34":
                    hdnReportId.Value = "e99f8a2b-8126-4d65-8e71-d1455654da64";
                    break;
                default:
                    hdnReportId.Value = "84c42a4f-e08f-446a-9e62-b5d14454f5fb";
                    break;
            }
            if (hdnReportId.Value != "")
            {
                clsADUserInfo obj = new clsADUserInfo();
                if (Session["token"] == null && IsRslapplied==true)
                {
                    string accesstoken = obj.GetApplicationToken();
                    if (accesstoken.Split('|')[0] == "2")
                    {
                        embedContainer.InnerHtml = "<pre>" + accesstoken.Split('|')[1] + "</pre>";
                        return;
                    }
                }
                else
                {
                    string accesstoken = obj.fnGetTokenNoForUser();
                    if (accesstoken.Split('|')[0] == "2")
                    {
                        embedContainer.InnerHtml = "<pre>" + accesstoken.Split('|')[1] + "</pre>";
                        return;
                    }
                }
                if (Session["token"] != null)
                {
                    string str = obj.GetEmbedUrl(hdnReportId.Value, Convert.ToString(Session["token"]));
                    if (str.Split('|')[0] == "1")
                    {
                        hdnEmbedUrl.Value = str.Split('|')[1];
                        string Username = Session["username"].ToString();
                        string str1 = obj.fngeneratetoken(hdnReportId.Value, Convert.ToString(Session["token"]), Username, str.Split('|')[2], IsRslapplied);
                        if (str1.Split('|')[0] == "1")
                        {
                            hdnEmbedaccesstoken.Value = str1.Split('|')[1];
                        }
                        else
                        {
                            embedContainer.InnerHtml = "<pre>" + str1.Split('|')[1] + "</pre>";
                        }
                    }
                    else
                    {
                        embedContainer.InnerHtml = "<pre>" + str.Split('|')[1] + "</pre>";
                    }
                }
            }
            else
            {
                embedContainer.InnerHtml = "Comming Soon..";
            }
        }
    }

  
}