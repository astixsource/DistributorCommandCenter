using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using X15 = DocumentFormat.OpenXml.Office2013.Excel;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using A = DocumentFormat.OpenXml.Drawing;
using Ap = DocumentFormat.OpenXml.ExtendedProperties;
using Vt = DocumentFormat.OpenXml.VariantTypes;
using Thm15 = DocumentFormat.OpenXml.Office2013.Theme;
using System.Globalization;
public static class TypeHelper
{
    private static readonly HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float)
        };

    public static bool IsNumeric(this Type myType)
    {
        return NumericTypes.Contains(Nullable.GetUnderlyingType(myType) ?? myType);
    }
}
public partial class frmDownloadExcel: System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        //Response.Write("<br/><br/><b>Report is being downloaded,please wait... </b>");
        string[] notrowspanColumn = new string[0];
        string sFlg = Request.QueryString["flg"].ToString();//"1";//
        if (sFlg == "1")
        {
            fnDownloadJVReport();
        }else if (sFlg == "2")
        {
            fnDownloadJVHistory();
        }
        else if (sFlg == "3")
        {
            fnDownloadJVProductivityReport();
        }
        else if (sFlg == "4")
        {
            fnDownloadManagePostingData();
        }
        else if (sFlg == "5")
        {
            string jvids = Request.QueryString["JVIDs"] == null ? "" : Request.QueryString["JVIDs"].ToString();
            fnDownloadJBInBulkData(jvids);
        }
        else if (sFlg == "6")
        {
            fnDownloadJVGetCycleReport();
        }
        else if (sFlg == "7")
        {
            fnDownloadJVSummaryReport();
        }
        else if (sFlg == "8")
        {
            fnDownloadJVListingForValidatorReport();
        }
        else if (sFlg == "9")
        {
            string monthyear = Request.QueryString["monthyear"] == null ? "" : Request.QueryString["monthyear"].ToString();
string LoginId = Request.QueryString["LoginId"] == null ? "0" : Request.QueryString["LoginId"].ToString();
string RoleID = Request.QueryString["RoleID"] == null ? "0" : Request.QueryString["RoleID"].ToString();
string UserID= Request.QueryString["UserID"] == null ? "0" : Request.QueryString["UserID"].ToString();
            fnRptSLAReportforCG(monthyear,LoginId,RoleID,UserID);
        }
    }


    public void fnRptSLAReportforCG(string monthyear,string LoginID,string RoleID,string UserID)
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "RptSLAReportforCG_" + monthyear + "_"+ DateTime.Now.ToString("yyyyMMdd");
        try
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            string storedProcName = "spRptSLAReportforCG";
            List<SqlParameter> sp = new List<SqlParameter>()
                    {
                 new SqlParameter("@MOnthVal", monthyear.Split('_')[0]),
                   new SqlParameter("@YEarVal",  monthyear.Split('_')[1]),
                   new SqlParameter("@LoginID",  LoginID),
                   new SqlParameter("@RoleID", RoleID),
                   new SqlParameter("@UserID",  UserID),
                };
            DataSet Ds = clsDbCommand.ExecuteQueryReturnDataSet(storedProcName, con, sp);
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Tables[resulsetcnt];
                //DataRow[] selected = dt.Select("[Current Status] like '%<b>%'");
                //foreach (DataRow row in selected)
                //{
                //    row["Current Status"] = row["Current Status"].ToString().Replace("<b>", "").Replace("</b>", "");
                //}
                //dt.AcceptChanges();
                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());

                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                //ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }
    public void fnDownloadJVListingForValidatorReport()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVListingForValidatorReport_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataSet Ds = (DataSet)Session["dsJVGetJVListingForValidator"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Copy().Tables[resulsetcnt];
                dt.Columns.Remove("JVID");
                dt.Columns.Remove("WorkflowID");
                dt.Columns.Remove("flgDetail");

                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/


               
                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());
                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                ws.Range(ws.Cell(k, 7), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                //ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }

    public void fnDownloadJVSummaryReport()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVSummaryReport_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataSet Ds = (DataSet)Session["dsJVSummaryReport"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Copy().Tables[resulsetcnt];
                 dt.Columns.Remove("FunctionID");
                dt.Columns.Remove("LocationNodeID");

                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

               
                k = 2;j = 0;
                for(var r = 0; r < dt.Rows.Count; r++)
                {
                    j = 0;
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                        {
                            if (Ds.Tables[0].Rows[r][c].ToString().Split('^').Length > 1)
                            {
                                ws.Cell(k + r, j + 1).Value = Convert.ToString(dt.Rows[r][dt.Columns[c].ColumnName.ToString()]).Split('^')[0] == "ZZZTotal" ? "Total" : Convert.ToString(dt.Rows[r][dt.Columns[c].ColumnName.ToString()]).Split('^')[1];
                            }
                            else
                            {
                                ws.Cell(k + r, j + 1).Value = Convert.ToString(dt.Rows[r][dt.Columns[c].ColumnName.ToString()]).Split('^')[0] == "ZZZTotal" ? "Total" : Convert.ToString(dt.Rows[r][dt.Columns[c].ColumnName.ToString()]).Split('^')[0];
                            }
                            if (j > 1)
                            {
                                ws.Cell(k + r, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            }
                            else
                            {
                                ws.Cell(k + r, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            }
                            j++;
                        }
                    }
                }
                ws.Rows().AdjustToContents();
               // var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());
                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                //ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }

    public void fnDownloadJVGetCycleReport()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVLifeCycleReport_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataSet Ds = (DataSet)Session["dsJVGetCycleReport"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Copy().Tables[resulsetcnt];
                dt.Columns.Remove("JVID");
                dt.Columns.Remove("WorkflowID");

                DataRow[] selected = dt.Select("[Current Status] like '%<b>%'");
                foreach (DataRow row in selected)
                {
                    row["Current Status"] = row["Current Status"].ToString().Replace("<b>","").Replace("</b>","");
                }
                dt.AcceptChanges();
                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());

                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                //ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }

    public void fnDownloadJVHistory()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVHistory_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataSet Ds = (DataSet)Session["dsJVHistory"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Tables[resulsetcnt];
                DataRow[] selected = dt.Select("[Current Status] like '%<b>%'");
                foreach (DataRow row in selected)
                {
                    row["Current Status"] = row["Current Status"].ToString().Replace("<b>", "").Replace("</b>", "");
                }
                dt.AcceptChanges();
                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());

                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                //ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }
   

    public void fnDownloadJVReport()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVReport_" + DateTime.Now.ToString("yyyyMMdd");


        try
        {
            DataSet Ds = (DataSet)Session["dsJVReport"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Tables[resulsetcnt];
                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());

                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
               // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                //ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }

    public void fnDownloadJVProductivityReport()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVProductivityReport_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataSet Ds = (DataSet)Session["dsProductivityReport"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "Sheet1";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Tables[resulsetcnt];
                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());

                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
                ws.Column(1).Delete();


                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }

    public void fnDownloadManagePostingData()
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "ManagePostingData_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataSet Ds = (DataSet)Session["dsManagePosting"];
            using (XLWorkbook wb = new XLWorkbook())
            {
                cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                string strSheetName = "JV";//drowchasiss["SheetName"].ToString();
                DataTable dt = Ds.Copy().Tables[resulsetcnt];
                dt.Columns.Remove("JVID");
                dt.Columns.Remove("WorkflowID");
                dt.Columns.Remove("Download Excel");
                dt.Columns.Remove("Comments");
                resulsetcnt++;
                var ws = wb.Worksheets.Add(strSheetName);
                k = 1; j = 0; colFreeze = 2; colLeft = 3;
                strold = ""; cntc = 0; colst = 2; flgb = true; bool flgm = false;
                //int rowstart = 0; // for data part insertion
                int noofsplit = 1; //Convert.ToInt16(drowchasiss["NoOfSplit"]);
                int noofcolfreeze = 1;// Convert.ToInt16(drowchasiss["Noofcolfreeze"]);
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    if (!SkipColumn.Contains(dt.Columns[c].ColumnName.ToString().Trim()))
                    {
                        string[] ColSpliter = dt.Columns[c].ColumnName.ToString().Split('^');


                        flgm = true;

                        for (var i = 0; i < ColSpliter.Length; i++)
                        {
                            string sVal = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                            ws.Cell(k + i, j + 1).Value = sVal.Split('^')[0];
                        }
                        for (var i = 0; i < noofsplit; i++)
                        {
                            string bgcolor = "#728cd4"; string forrecolor = "#ffffff";
                            if (i == 1)
                            {
                                bgcolor = "#a4b6e3";
                                forrecolor = "#000000";
                            }
                            ws.Cell(k + i, j + 1).Style.Fill.BackgroundColor = XLColor.FromHtml(bgcolor);
                            ws.Cell(k + i, j + 1).Style.Font.FontColor = XLColor.FromHtml(forrecolor);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Cell(k + i, j + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        }
                        j++;
                    }
                }

                for (var i = 0; i < noofsplit - 1; i++)
                {
                    j = 0; colst = 1; k = 1; strold = "";
                    for (int c = 0; c < dt.Columns.Count; c++)
                    {
                        //if (strold != "")
                        //{
                        if (strold != dt.Columns[c].ColumnName.ToString().Split('^')[i])
                        {
                            flgb = true;
                            if (strold != "")
                            {
                                ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j)).Merge();
                            }
                            cntc = 0;
                        }
                        //}
                        if (flgb == true)
                        {
                            colst = j + 1;
                        }
                        flgb = false;
                        strold = dt.Columns[c].ColumnName.ToString().Split('^')[i];
                        cntc++;
                        if (c == dt.Columns.Count - 1)
                        {
                            ws.Range(ws.Cell(k + i, colst), ws.Cell(k + i, j + 1)).Merge();
                            cntc = 0;
                        }

                        j++;
                    }
                }


                int rowst = 0;
                for (int c = 0; c < dt.Columns.Count; c++)
                {
                    strold = dt.Columns[c].ColumnName.ToString().Split('^')[0];
                    colst = 1; k = 1; flgb = false; rowst = 1;


                    for (var i = 0; i < noofsplit; i++)
                    {
                        //strold = "";                                                   
                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i, c + 1)).Merge();
                            flgb = false;
                            rowst++;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] == "")
                        {
                            flgb = true;
                        }

                        if (dt.Columns[c].ColumnName.ToString().Split('^')[i] != "" && flgb == false && i > 0)
                        {
                            rowst++;
                        }

                        if (i == noofsplit - 1 && flgb == true)
                        {
                            ws.Range(ws.Cell(rowst, c + 1), ws.Cell(i + 1, c + 1)).Merge();
                        }
                    }
                }
                /**/

                ws.Rows().AdjustToContents();
                var rangeWithData = ws.Cell(noofsplit + 1, 1).InsertData(dt.AsEnumerable());

                //ws.Columns().AdjustToContents();//noofsplit + 1,  dt.Columns.Count

                IXLCell cell3 = ws.Cell(1, 1);
                IXLCell cell4 = ws.Cell(dt.Rows.Count + noofsplit, dt.Columns.Count);
                //ws.Range(ws.Cell(k, 2), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 8), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 9), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                //ws.Range(ws.Cell(k, 10), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 11), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 12), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                //ws.Range(ws.Cell(k, 13), cell4).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Range(cell3, cell4).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(cell3, cell4).Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Fill.BackgroundColor = XLColor.FromHtml("#d6d6d6");
                //ws.Range(ws.Cell(dt.Rows.Count + 1, 1), cell4).Style.Font.FontColor = XLColor.FromHtml("#000000");
                ws.SheetView.FreezeRows(noofsplit);
                ws.SheetView.FreezeColumns(noofcolfreeze);
                //}
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
                ws.Range(1, 1, 1, dt.Columns.Count).Style.Alignment.WrapText = true;
                // ws.Cell(dt.Rows.Count + 1, 1).Value = "";
              

                //Export the Excel file.
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                //Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.Response.End();
                }
            }
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }

    public void fnDownloadJBInBulkData(string jvids)
    {
        string[] SkipColumn = new string[0];
        string filename = "";
        int cntvalid = 0;
        filename = "JVInBulkFile_" + DateTime.Now.ToString("yyyyMMdd");
        try
        {
            DataTable udt_JVID = new DataTable();
            udt_JVID.Columns.Add(new DataColumn("JVID", typeof(string)));
            for(int i = 0; i < jvids.Split(',').Length; i++)
            {
                DataRow drow = udt_JVID.NewRow();
                drow[0] = jvids.Split(',')[i];
                udt_JVID.Rows.Add(drow);
            }
            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spJVDownloadInBulkJVFile_download]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.CommandTimeout = 0;
            Scmd.Parameters.AddWithValue("@JVID", udt_JVID);

            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataSet Ds = new DataSet();
            Sdap.Fill(Ds);

            cntvalid = 1;
                ////Start Chassiss
                int k = 1; int j = 0; int colFreeze = 2; int colLeft = 3;
                string strold = ""; int cntc = 0; int colst = 2; bool flgb = true;
                int resulsetcnt = 0;
                //foreach (DataRow drowchasiss in Ds.Tables[0].Rows)//For SheetName
                //{
                DataTable dt = Ds.Tables[resulsetcnt];
               
                resulsetcnt++;

            cntvalid = 1;
            var delimiter = "\t";

            MemoryStream ms = new MemoryStream();
            TextWriter tw = new StreamWriter(ms);
          
            foreach (DataRow drow in dt.Rows)
            {
                StringBuilder sb = new StringBuilder();
                foreach (DataColumn dcolName in dt.Columns)
                {
                    if (sb.ToString() == "")
                    {
                        sb.Append(drow[dcolName.ColumnName]);
                    }
                    else
                    {
                        sb.Append("\t" + drow[dcolName.ColumnName]);
                    }
                    //var line = string.Join(delimiter, Convert.ToString(drow[dcolName.ColumnName]));
                }
                tw.WriteLine(sb.ToString());
            }

            tw.Flush();
            byte[] bytes = ms.ToArray();
            ms.Close();

            Response.Clear();
            Response.ContentType = "application/force-download";
            Response.AddHeader("content-disposition", "attachment;    filename=" + filename + ".txt");
            Response.BinaryWrite(bytes);
            Response.End();
        }
        catch (Exception ex)
        {
            if (cntvalid == 0)
            {
                Response.Write(ex.Message);
            }
            // string ProjectTitle = ConfigurationManager.AppSettings["Title"];
            //clsSendLogMail.fnSendLogMail(ex.Message, ex.ToString(), "FrmDownload Page", "Download Page", "Error in FrmDownload Page in " + ProjectTitle);
        }
        finally
        {
        }
    }
}