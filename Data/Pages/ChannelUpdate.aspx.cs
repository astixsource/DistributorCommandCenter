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


public partial class HawkeyeSubDTarget : System.Web.UI.Page
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

            MonthYearDDL();
        }
    }
    private void MonthYearDDL()
    {
        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandTimeout = 0;
        Scmd.CommandText = "[spGetMonths]";
        Scmd.Parameters.AddWithValue("@UserID", hdnUserId.Value);
        Scmd.Parameters.AddWithValue("@LoginID", hdnLoginId.Value);
        Scmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet Ds = new DataSet();
        Sdap.Fill(Ds);

        int selectedIndex = 0;
        for (int i = 0; i < Ds.Tables[0].Rows.Count; i++)
        {
            ddlMonthYear.Items.Add(new ListItem(Ds.Tables[0].Rows[i]["MOnthYear"].ToString(), Ds.Tables[0].Rows[i]["MonthVal"].ToString() + "|" + Ds.Tables[0].Rows[i]["YearVal"].ToString() + "|" + Ds.Tables[0].Rows[i]["AllowedDate"].ToString()));

            if (Ds.Tables[0].Rows[i]["flgSelect"].ToString() == "1")
                selectedIndex = i;
        }

        ddlMonthYear.SelectedIndex = selectedIndex;
    }


    [System.Web.Services.WebMethod()]
    public static string GetReport(string UserID, string MonthVal, string YearVal, string LoginID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spChannelChangeGetDataForCommit]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@MonthVal", MonthVal);
            Scmd.Parameters.AddWithValue("@YearVal", YearVal);
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn = new string[1];
            SkipColumn[0] = "ChannelChangeBranchDataID";

            return "0|^|" + CreateHtmlTbl(Ds.Tables[0], SkipColumn, "BranchData");
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
        sb.Append("<th><input type='checkbox' onclick='fnCheckAll(this);'/></th>");
        for (int j = 0; j < dt.Columns.Count; j++)
            if (!SkipColumn.Contains(dt.Columns[j].ColumnName))
            {
                if (dt.Columns[j].ColumnName.ToString() == "flgComitButton")
                    sb.Append("<th>Action</th>");
                else
                    sb.Append("<th>" + dt.Columns[j].ColumnName + "</th>");
            }

        sb.Append("</tr>");
        sb.Append("</thead>");

        sb.Append("<tbody>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr flgEdit='0' strId='" + dt.Rows[i]["ChannelChangeBranchDataID"].ToString() + "'>");

            if (dt.Rows[i]["flgComitButton"].ToString().Trim() == "1")
                sb.Append("<td><input type='checkbox' onclick='fnCheckIndividual(this);'/></td>");
            else
                sb.Append("<td></td>");
            //sb.Append("<td>" + (i + 1).ToString() + "</td>");

            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim()))
                {
                    if (dt.Columns[j].ColumnName.ToString().Trim() == "flgComitButton")
                    {
                        if (dt.Rows[i][j].ToString().Trim() == "2")
                            sb.Append("<td><a href='#' onclick='fnViewBranchData(this);'>View</a></td>");
                        else if (dt.Rows[i][j].ToString().Trim() == "1")
                            sb.Append("<td><a href='#' onclick='fnCommitBranchData(this);'>Commit</a></td>");
                        else
                            sb.Append("<td></td>");
                    }
                    else
                        sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");
                }

            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }


    [System.Web.Services.WebMethod()]
    public static string BranchWiseReport(string UserID, string MonthVal, string YearVal, string LoginID, string ChannelChangeBranchDataID)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spGetChannelChangeLoadedData]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@MonthVal", MonthVal);
            Scmd.Parameters.AddWithValue("@YearVal", YearVal);
            Scmd.Parameters.AddWithValue("@ChannelChangeBranchDataID", ChannelChangeBranchDataID);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn = new string[0];
            return "0|^|" + CreateBranchHtmlTbl(Ds.Tables[0], SkipColumn, "BranchWiseData");
        }
        catch (Exception ex)
        {
            return "1|^|" + ex.Message;
        }
    }
    private static string CreateBranchHtmlTbl(DataTable dt, string[] SkipColumn, string tblname)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table id='tbl" + tblname + "' class='table table-sm table-bordered mb-0 cls-" + tblname + "'>");
        sb.Append("<thead>");
        sb.Append("<tr>");
        sb.Append("<th>#</th>");
        for (int j = 0; j < dt.Columns.Count; j++)
            if (!SkipColumn.Contains(dt.Columns[j].ColumnName))
                sb.Append("<th>" + dt.Columns[j].ColumnName + "</th>");

        sb.Append("</tr>");
        sb.Append("</thead>");

        sb.Append("<tbody>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr>");
            sb.Append("<td>" + (i + 1).ToString() + "</td>");
            for (int j = 0; j < dt.Columns.Count; j++)
                if (!SkipColumn.Contains(dt.Columns[j].ColumnName.ToString().Trim()))
                    sb.Append("<td>" + dt.Rows[i][j].ToString() + "</td>");

            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }

    [System.Web.Services.WebMethod()]
    public static string CommitBranchData(string ChannelChangeBranchDataID, string UserID, string MonthVal, string YearVal, string LoginID)
    {
        try
        {
            for (int i = 0; i < ChannelChangeBranchDataID.Split('^').Length; i++)
            {
                if (ChannelChangeBranchDataID.Split('^')[i] != "")
                {
                    SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
                    SqlCommand Scmd = new SqlCommand();
                    Scmd.Connection = Scon;
                    Scmd.CommandText = "[spChannelChanelCommitData]";
                    Scmd.CommandType = CommandType.StoredProcedure;
                    Scmd.CommandTimeout = 0;
                    Scmd.Parameters.AddWithValue("@ChannelChangeBranchDataID", ChannelChangeBranchDataID.Split('^')[i]);
                    Scmd.Parameters.AddWithValue("@UserID", UserID);
                    Scmd.Parameters.AddWithValue("@LoginID", LoginID);
                    //Scmd.Parameters.AddWithValue("@MonthVal", MonthVal);
                    //Scmd.Parameters.AddWithValue("@YearVal", YearVal);
                    SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
                    DataSet Ds = new DataSet();
                    Sdap.Fill(Ds);
                }
            }

            return "0|^|";
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
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spGetChannelChangeLoadedData]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@LoginID", hdnLoginId.Value);
            Scmd.Parameters.AddWithValue("@UserID", hdnUserId.Value);
            Scmd.Parameters.AddWithValue("@MonthVal", ddlMonthYear.SelectedValue.Split('|')[0]);
            Scmd.Parameters.AddWithValue("@YearVal", ddlMonthYear.SelectedValue.Split('|')[1]);
            Scmd.Parameters.AddWithValue("@ChannelChangeBranchDataID", hdnChannelChangeBranchDataID.Value);
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
    protected void btnDownloadErrorRpt_Click(object sender, EventArgs e)
    {
        string[] SkipColumn = new string[0];
        DataTable dt = (DataTable)Session["dtError"];
        string filename = "Channel Updating Error Report dated " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

        XLWorkbook wb = new XLWorkbook();
        wb = AddWorkSheet(wb, dt, SkipColumn, "Error Report");
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