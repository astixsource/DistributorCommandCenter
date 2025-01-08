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
        //Current Month-Year +/- 2
        for (int i=0; i < 9; i++)
            ddlMonthYear.Items.Add(new ListItem(DateTime.Now.AddMonths(i - 6).ToString("MMM-yyyy"), DateTime.Now.AddMonths(i - 6).ToString("MM-yyyy")));

        ddlMonthYear.SelectedIndex = 6;
    }

    protected void btnDownloadTemplate_Click(object sender, EventArgs e)
    {
        try
        {
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spSubdHawkeyeGetSubDTargetData]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@NodeID", hdnNodeID.Value);
            Scmd.Parameters.AddWithValue("@UserID", hdnUserId.Value);
            Scmd.Parameters.AddWithValue("@MonthVal", ddlMonthYear.SelectedValue.Split('-')[0]);
            Scmd.Parameters.AddWithValue("@YearVal", ddlMonthYear.SelectedValue.Split('-')[1]);
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            string[] SkipColumn = new string[0];

            DataTable dt = Ds.Tables[0];
            string filename = "SubD Target for " + ddlMonthYear.SelectedItem;

            XLWorkbook wb = new XLWorkbook();
            wb = AddWorkSheet(wb, dt, SkipColumn, "SubD Target - " + ddlMonthYear.SelectedItem);
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
                divMsg.InnerText = "Error : " + ex.Message;
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
                        ws.Cell(k, j).Style.Fill.BackgroundColor = XLColor.FromHtml("#" + dt.Rows[b][c].ToString().Split('^')[1]);
                    }
                    else
                        ws.Cell(k, j).Value = dt.Rows[b][c].ToString();
                }
            }
        }
        cellEnd = ws.Cell(k, j);

        ws.Range(cellStart, cellEnd).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Range(cellStart, cellEnd).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
        ws.Range(cellStart, cellEnd).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
        ws.Range(cellStart, cellEnd).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        ws.Range(cellStart, cellEnd).Style.Font.SetFontSize(10);
        ws.SheetView.FreezeRows(FreezeRows);
        //ws.Columns().Width = 20;

        ws.Column(1).Width = 20;
        ws.Column(2).Width = 30;
        ws.Column(3).Width = 10;
        ws.Column(4).Width = 10;
        ws.Column(5).Width = 14;
        ws.Range(ws.Cell(trCntr + 1, 3), ws.Cell(k, 3)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(ws.Cell(trCntr + 1, 4), ws.Cell(k, 4)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(ws.Cell(trCntr + 1, 5), ws.Cell(k, 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        ws.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        return wb;
    }
}