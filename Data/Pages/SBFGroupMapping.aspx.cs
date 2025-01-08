using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using Newtonsoft.Json;


public partial class SBFGroupMapping : System.Web.UI.Page
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
            hdnRoleId.Value = Session["RoleID"].ToString();
            hdnNodeID.Value = Session["NodeID"].ToString();
            hdnUserName.Value = Session["FullName"].ToString();

            Mstrs();
        }
    }
    private void Mstrs()
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);

        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandTimeout = 0;
        Scmd.CommandText = "[spSBFGroupGetProdHier]";
        Scmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet Ds = new DataSet();
        Sdap.Fill(Ds);
        Sdap.Dispose();
        Scmd.Dispose();


        ddlCategoryFilter.Items.Add(new ListItem("--Select--", "0"));
        for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
            ddlCategoryFilter.Items.Add(new ListItem(Ds.Tables[0].Rows[i]["Category"].ToString(), Ds.Tables[0].Rows[i]["CatNOdeID"].ToString()));

        hdnBrandMstr.Value = JsonConvert.SerializeObject(Ds.Tables[1], Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();
        hdnBrandFormMstr.Value = JsonConvert.SerializeObject(Ds.Tables[2], Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();


        Ds.Clear();
        Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandTimeout = 0;
        Scmd.CommandText = "[spSBFGroupMstr]";
        Scmd.CommandType = CommandType.StoredProcedure;
        Sdap = new SqlDataAdapter(Scmd);
        Ds = new DataSet();
        Sdap.Fill(Ds);

        hdnGroupMstr.Value = JsonConvert.SerializeObject(Ds.Tables[0], Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();
    }


    [System.Web.Services.WebMethod()]
    public static string GetReport(string LoginID, string UserID, string Category, string Brand, string BrandForm, string flgSBFType)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSBFGroupGetSBFToMapGroup]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@CategoryNodeID", Category);
            Scmd.Parameters.AddWithValue("@BrandNodeID", Brand);
            Scmd.Parameters.AddWithValue("@BrandFormNodeID", BrandForm);
            Scmd.Parameters.AddWithValue("@flgNew", flgSBFType);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn;
            if (flgSBFType == "1")
            {
                SkipColumn = new string[5];
                SkipColumn[0] = "BRANDNODEID";
                SkipColumn[1] = "NODEID";
                SkipColumn[2] = "SBFGROUPID";
                SkipColumn[3] = "CATNODEID";
                SkipColumn[4] = "STRSEARCH";
            }
            else
            {
                SkipColumn = new string[6];
                SkipColumn[0] = "BRANDNODEID";
                SkipColumn[1] = "NODEID";
                SkipColumn[2] = "SBFGROUPID";
                SkipColumn[3] = "CATNODEID";
                SkipColumn[4] = "LAST TRAN DATE";
                SkipColumn[5] = "STRSEARCH";
            }

            return "0|^|" + CreateHtmlTbl(Ds.Tables[0], SkipColumn, "SBFGrpMapping");

            //string GroupMstr = JsonConvert.SerializeObject(Ds.Tables[1], Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();
            //return "0|^|" + CreateHtmlTbl(Ds.Tables[0], SkipColumn, "SBFGrpMapping") + "|^|" + GroupMstr;
        }
        catch (Exception ex)
        {
            return "1|^|" + ex.Message;
        }
    }
    private static string CreateHtmlTbl(DataTable dt, string[] SkipColumn, string tblname)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table id='tbl" + tblname + "' class='table table-sm table-bordered mb-0 cls-" + tblname + "'>");
        sb.Append("<thead>");
        sb.Append("<tr>");

        sb.Append("<th>#</th>");
        //sb.Append("<th><input type='checkbox' onclick='fnCheckAll(this);'/></th>");
        for (int j = 0; j < dt.Columns.Count; j++)
            if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim().ToUpper()))
                sb.Append("<th>" + dt.Columns[j].ColumnName + "</th>");

        sb.Append("<th>Action</th>");
        sb.Append("<th style='display: none;'>Search</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");

        sb.Append("<tbody>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr CatNodeID='" + dt.Rows[i]["CatNodeID"].ToString() + "' brandId='" + dt.Rows[i]["BrandNodeID"].ToString() + "' sbfId='" + dt.Rows[i]["NodeID"].ToString() + "'>");

            sb.Append("<td>" + (i + 1).ToString() + "</td>");
            //sb.Append("<td><input type='checkbox' onclick='fnCheckIndividual(this);'/></td>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim().ToUpper()))
                {
                    if (dt.Columns[j].ColumnName.ToString().ToUpper() == "SBF GROUP")
                    {
                        sb.Append("<td>");
                        sb.Append("<div style='position: relative;'>");
                        sb.Append("<input type='text' class='w-100 form-control tbl-control-txt' selectedid='" + dt.Rows[i]["SBFGrouPID"] + "' value='" + dt.Rows[i][j] + "' onkeyup='fnShowGroupPopup(this);' onclick='fnShowGroupPopup(this);'>");
                        sb.Append("<div class='popup-content popup-content-grp'></div>");
                        sb.Append("</div>");
                        sb.Append("</td>");
                    }
                    else
                        sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");
                }

            sb.Append("<td><a href='#' onclick='fnUpdateMapping(this);'>Save</a></td>");
            sb.Append("<td iden='search' style='display: none;'>" + dt.Rows[i]["strSearch"].ToString() + "</td>");
            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }    


    [System.Web.Services.WebMethod()]
    public static string fnUpdateMapping(string strSBFGroiPMapping, string UserID, string LoginID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSBFGroupUpdateMapping]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@strSBFGroiPMapping", strSBFGroiPMapping);    //SBFNodeID^GroupID|
            Scmd.Parameters.AddWithValue("@UserID ", UserID);
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
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


    [System.Web.Services.WebMethod()]
    public static string fnAddEditGroup(string GroupID, string GroupCode, string GroupName, string RoleID, string LoginID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSBFGroupCreateGroup]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@GroupName ", GroupName);
            Scmd.Parameters.AddWithValue("@GroupID ", GroupID);
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            Scmd.Parameters.AddWithValue("@RoleID ", RoleID);
            Scmd.Parameters.AddWithValue("@GroupCode ", GroupCode);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);
            GroupID = Ds.Tables[0].Rows[0]["GroupID"].ToString();

            Ds.Clear();
            Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandTimeout = 0;
            Scmd.CommandText = "[spSBFGroupMstr]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Sdap = new SqlDataAdapter(Scmd);
            Ds = new DataSet();
            Sdap.Fill(Ds);

            string GrpMstr = JsonConvert.SerializeObject(Ds.Tables[0], Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }).ToString();

            return "0|^|" + GroupID + "|^|" + GroupName + "|^|" + GrpMstr;
        }
        catch (Exception ex)
        {
            return "1|^|" + ex.Message;
        }
    }


    protected void btnDownloadTemplate_Click(object sender, EventArgs e)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spGetChannelChangeLoadedData]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@LoginID", hdnLoginId.Value);
            Scmd.Parameters.AddWithValue("@UserID", hdnUserId.Value);
            //Scmd.Parameters.AddWithValue("@MonthVal", ddlMonthYear.SelectedValue.Split('|')[0]);
            //Scmd.Parameters.AddWithValue("@YearVal", ddlMonthYear.SelectedValue.Split('|')[1]);
            //Scmd.Parameters.AddWithValue("@ChannelChangeBranchDataID", hdnChannelChangeBranchDataID.Value);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            DataTable dt = Ds.Tables[0];
            string[] SkipColumn = new string[0];
            string filename = "Channel(s) dated " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

            XLWorkbook wb = new XLWorkbook();
            wb = AddWorkSheet(wb, dt, SkipColumn, "Channel");
            try
            {
                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
            catch (Exception ex)
            {
                //divMsg.InnerText = "Error : " + ex.Message;
            }
            finally
            {
                Sdap.Dispose();
                Scmd.Dispose();
                Scon.Dispose();
            }
        }
        catch (Exception ex)
        {
            //
        }
    }
    private static XLWorkbook AddWorkSheet(XLWorkbook wb, DataTable dt, string[] SkipColumn, string Sheetname)
    {
        IXLCell cellStart;
        IXLCell cellEnd;
        int k = 0, j = 0, trCntr = 0;
        var ws = wb.Worksheets.Add(Sheetname);

        //-----------Header------------------------
        k++;
        int FreezeRows = k;
        cellStart = ws.Cell(k, j + 1);
        for (int c = 0; c < dt.Columns.Count; c++)
        {
            if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
            {
                j++;
                ws.Cell(k, j).Value = dt.Columns[c].ColumnName.ToString();
                ws.Cell(k, j).Style.Alignment.WrapText = true;
                ws.Cell(k, j).Style.Fill.BackgroundColor = XLColor.FromHtml("#728cd4");
                ws.Cell(k, j).Style.Font.FontColor = XLColor.FromHtml("#ffffff");
                ws.Cell(k, j).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            }
        }

        //------------Body---------------------------
        trCntr = k;
        for (int b = 0; b < dt.Rows.Count; b++)
        {
            k++; j = 0;
            for (int c = 0; c < dt.Columns.Count; c++)
            {
                if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                {
                    j++;
                    ws.Cell(k, j).Style.Alignment.WrapText = true;
                    if (dt.Rows[b][c].ToString().Split('^').Length > 1)
                    {
                        ws.Cell(k, j).Value = dt.Rows[b][c].ToString().Split('^')[0];
                        ws.Cell(k, j).Style.Fill.BackgroundColor = XLColor.FromHtml("#FF0000");
                        //ws.Cell(k, j).Style.Fill.BackgroundColor = XLColor.FromHtml("#" + dt.Rows[b][c].ToString().Split('^')[1]);
                    }
                    else
                        ws.Cell(k, j).Value = dt.Rows[b][c].ToString();
                }
            }
        }
        cellEnd = ws.Cell(k, j);

        ws.Range(cellStart, cellEnd).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        ws.Range(cellStart, cellEnd).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        ws.Range(cellStart, cellEnd).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
        ws.Range(cellStart, cellEnd).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        ws.Range(cellStart, cellEnd).Style.Font.SetFontSize(10);
        ws.SheetView.FreezeRows(FreezeRows);
        ws.Columns().Width = 20;

        //ws.Column(1).Width = 20;
        //ws.Column(2).Width = 30;
        //ws.Column(3).Width = 10;
        //ws.Column(4).Width = 10;
        //ws.Column(5).Width = 14;
        //ws.Range(ws.Cell(trCntr + 1, 3), ws.Cell(k, 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //ws.Range(ws.Cell(trCntr + 1, 4), ws.Cell(k, 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        //ws.Range(ws.Cell(trCntr + 1, 5), ws.Cell(k, 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        //ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        return wb;
    }
}