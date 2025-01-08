    <%@ WebHandler Language="C#" Class="FileUpload" %>

using System;
using System.IO;
using System.Web;
using System.Data;
using System.Linq;
using ClosedXML.Excel;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using ExcelDataReader;

public class FileUpload : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var postedFile = context.Request.Files[0];
        string FileType = context.Request.Form["FileType"].ToString();
        string LoginID = context.Request.Form["LoginID"].ToString();
        string UserID = context.Request.Form["UserID"].ToString();
        string MonthYr = context.Request.Form["MonthYr"].ToString();
        try
        {
            string msg = UploadFile(postedFile, FileType, LoginID, UserID, MonthYr, context);
            context.Response.Write(msg);
        }
        catch (Exception ex)
        {
            //SendMail(ConfigurationManager.AppSettings["MailTO"].ToString(), "Hawkeye Phase : Error while Uploading File", "Error while Uploading File :<br/>File Type : " + FileSetType + "<br/>File Name : " + Path.GetFileName(postedFile.FileName) + "<br/>Date-Time : " + DateTime.Now.ToString() + "<br/>Error : " + ex.Message, "");
            //context.Response.Write("1^Due to some technical reasons, we are unable to process your request. Error : " + ex.Message + " !");

            context.Response.Write("1^" + ErrorToDatatableStr("Other", ex.Message));
        }
    }


    private string UploadFile(HttpPostedFile File_Up, string FileType, string LoginID, string UserID, string MonthYr, HttpContext context)
    {
        try
        {
            string FileExt = Path.GetExtension(File_Up.FileName).Split('.')[1].ToUpper();
            if (FileExt != "XLSX")
                return "1^" + ErrorToDatatableStr("Invalid File", "File must be of XLSX extension");

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Channel"));
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp"));
            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel/Error")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/Channel/Error"));

            //Save the uploaded file at new location.
            string filename = "Channel_" + LoginID + "_" + UserID + "_" + MonthYr.Replace('|', '_') + "_" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + Path.GetExtension(File_Up.FileName);
            string filePath = HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename;
            File_Up.SaveAs(filePath);

            //Read the Uploaded Excel & convert it into Dataset
            DataSet DsExcelData = new DataSet();
            using (FileStream oStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader iExcelDataReader = null;
                string extension = Path.GetExtension(filePath);
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = false
                    }
                };
                if (extension == ".xls" || extension == ".xlsb")
                {
                    iExcelDataReader = ExcelReaderFactory.CreateBinaryReader(oStream);
                }
                else if (extension == ".xlsx")
                {
                    iExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(oStream);
                }
                DsExcelData = iExcelDataReader.AsDataSet(conf);
            }

            if (DsExcelData.Tables[0].Rows.Count > 2000)
                return "1^You can only allow to upload Data upto 2000 records ! ";


            SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConnDCC"]);
            SqlCommand Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spGetWorkFlow]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@Descr", "Channel Change");
            Scmd.CommandTimeout = 0;
            SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
            DataTable dt = new DataTable();
            Sdap.Fill(dt);
            Scmd.Dispose();
            Sdap.Dispose();

            string WorkflowID = dt.Rows[0]["WorkflowID"].ToString();
            dt.Dispose();

            Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spFileLoadingGetExcelReadingCell]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@WorkflowID", WorkflowID);
            Scmd.CommandTimeout = 0;
            Sdap = new SqlDataAdapter(Scmd);
            DataSet ds = new DataSet();
            Sdap.Fill(ds);
            Scmd.Dispose();
            Sdap.Dispose();

            DataTable dtExcelData = DsExcelData.Tables[0];
            string[] strAlphaArr = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH" };
            string strValidate = ValidateExcel(dtExcelData, ds.Tables[0].Rows[0]["strValidation"].ToString(), strAlphaArr);
            if (strValidate != "0")
                return strValidate;

            DataTable strTblValue = GetTableValue(dtExcelData, Convert.ToInt32(ds.Tables[0].Rows[0]["DataStartRowNo"]), ds.Tables[0].Rows[0]["strData"].ToString(), strAlphaArr);


            ds.Dispose();
            ds = new DataSet();
            Scmd = new SqlCommand();
            Scmd.Connection = Scon;
            Scmd.CommandText = "[spFileLoading]";
            Scmd.CommandType = CommandType.StoredProcedure;
            Scmd.Parameters.AddWithValue("@WorkflowID", WorkflowID);
            Scmd.Parameters.AddWithValue("@LoginID", LoginID);
            Scmd.Parameters.AddWithValue("@FileName", filename);
            Scmd.Parameters.AddWithValue("@ExcelData", strTblValue);
            Scmd.Parameters.AddWithValue("@UserID", UserID);
            Scmd.Parameters.AddWithValue("@MonthVal", MonthYr.Split('|')[0]);
            Scmd.Parameters.AddWithValue("@YearVal", MonthYr.Split('|')[1]);
            Scmd.CommandTimeout = 0;
            Sdap = new SqlDataAdapter(Scmd);
            Sdap.Fill(ds);
            Scmd.Dispose();
            Sdap.Dispose();

            // -1: Error while reading the File; 0: Incorrect Data like Dist Codes, Barnch Codes, etc; 1: Success
            if (ds.Tables[0].Rows[0]["flgSucess"].ToString() == "1")
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename))
                    File.Move(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename, HttpContext.Current.Server.MapPath("~/Files/Channel/") + filename);

                if (ds.Tables.Count > 2)
                {
                    if (ds.Tables[2].Rows.Count > 0)
                        return "0^<div style='margin: 10px 0; font-weight: 600;'>Channel Updated successfully at " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + ", except for the following Branch(s) :</div>" + DatatabletoStr(ds.Tables[2], "OtherUserBranch") + "^" + ds.Tables[1].Rows[0]["FileID"].ToString();
                    else
                        return "0^Channel Updated successfully at " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + ".^" + ds.Tables[1].Rows[0]["FileID"].ToString();
                }
                else
                    return "0^Channel Updated successfully at " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + ".^" + ds.Tables[1].Rows[0]["FileID"].ToString();
            }
            else if (ds.Tables[0].Rows[0]["flgSucess"].ToString() == "-1")
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename))
                    File.Move(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename, HttpContext.Current.Server.MapPath("~/Files/Channel/Error/") + filename);

                return "1^Error while reading. Kindly re-check the File..";
            }
            else if (ds.Tables[0].Rows[0]["flgSucess"].ToString() == "0")
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename))
                    File.Move(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename, HttpContext.Current.Server.MapPath("~/Files/Channel/Error/") + filename);

                HttpContext.Current.Session["dtError"] = ds.Tables[2];
                return "2^Error while validating the Data. Please review the Downloaded File for error.";
            }
            else
            {
                if (File.Exists(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename))
                    File.Move(HttpContext.Current.Server.MapPath("~/Files/Channel/Temp/") + filename, HttpContext.Current.Server.MapPath("~/Files/Channel/Error/") + filename);

                return "3^" + DatatabletoStr(ds.Tables[1], "Error");
            }
        }
        catch (Exception ex)
        {
            //SendMail(ConfigurationManager.AppSettings["MailTO"].ToString(), "Hawkeye Phase : Error while Uploading File", "Error while Uploading File :<br/>File Type : " + FileSetType + "<br/>File Name : " + Path.GetFileName(File_Up.FileName) + "<br/>Date-Time : " + DateTime.Now.ToString() + "<br/>Error : " + ex.Message, "");
            return "1^" + ErrorToDatatableStr("Other", ex.Message);
            //return "1^Due to some technical reasons, we are unable to process your request. Error : " + ex.Message + " !";
        }
    }

    public string ValidateExcel(DataTable dt, string strValidation, string[] strAlphaArr)
    {
        string[] ColLst = strValidation.Split('|');
        for (int i = 0; i < ColLst.Length; i++)
        {
            if (ColLst[i] == "")
            {
                continue;
            }
            if (dt.Rows[Convert.ToInt32(ColLst[i].Split('^')[0]) - 1][Convert.ToInt32(ColLst[i].Split('^')[1]) - 1].ToString().Trim() != ColLst[i].Split('^')[2].ToString().Trim())
                return "1^" + ErrorToDatatableStr("Invalid Value", "Header/label at " + strAlphaArr[Convert.ToInt32(ColLst[i].Split('^')[0]) - 1] + ColLst[i].Split('^')[1] + " ( " + ColLst[i].Split('^')[2] + " ) is not matched.");
        }

        return "0";
    }

    public string GetHeaderValue(DataTable dt, string strHeader, string[] strAlphaArr)
    {
        StringBuilder sb = new StringBuilder();
        string[] HeaderLst = strHeader.Split('|');
        for (int i = 0; i < HeaderLst.Length; i++)
        {
            sb.Append(strAlphaArr[Convert.ToInt32(HeaderLst[i].Split('^')[0]) - 1] + HeaderLst[i].Split('^')[1] + "^" + dt.Rows[Convert.ToInt32(HeaderLst[i].Split('^')[1]) - 1][Convert.ToInt32(HeaderLst[i].Split('^')[0]) - 1].ToString().Trim() + "|");
        }

        return sb.ToString();
    }

    public DataTable GetTableValue(DataTable dtExceldata, int StartRowNo, string strColumn, string[] strAlphaArr)
    {
        int RowCntr = 0;
        int LastRowNo = dtExceldata.Rows.Count;

        DataTable dt = new DataTable();
        dt.Columns.Add("col1", typeof(Int32));
        dt.Columns.Add("col2", typeof(string));

        int IsBlankRow = 0, EmptyRowCount = 0;
        string[] ColumnLst = strColumn.Split('|');
        StringBuilder sbColValue = new StringBuilder();

        for (int r = StartRowNo - 1; r < LastRowNo; r++)
        {
            IsBlankRow = 0;
            RowCntr++;
            sbColValue.Clear();
            for (int i = 0; i < ColumnLst.Length; i++)
            {
                sbColValue.Append(strAlphaArr[Convert.ToInt32(ColumnLst[i]) - 1] + "^" + dtExceldata.Rows[r][Convert.ToInt32(ColumnLst[i]) - 1].ToString().Trim() + "|");
                if (dtExceldata.Rows[r][Convert.ToInt32(ColumnLst[i]) - 1].ToString().Trim() != "")
                {
                    IsBlankRow = 1;
                }
            }

            if (IsBlankRow == 0)
            {
                if (EmptyRowCount == 10)
                    break;
                else
                    EmptyRowCount++;
            }
            else
            {
                EmptyRowCount = 0;

                DataRow dr = dt.NewRow();
                dr[0] = RowCntr;
                dr[1] = sbColValue;
                dt.Rows.Add(dr);
            }
        }

        return dt;
    }

    public static string DatatabletoStr(DataTable dt, string tblname)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table id='tbl" + tblname + "' class='table table-striped table-bordered table-sm cls-" + tblname + "'>");
        sb.Append("<thead>");
        sb.Append("<tr>");
        for (int j = 0; j < dt.Columns.Count; j++)
        {
            sb.Append("<th>" + dt.Columns[j].ColumnName.ToString() + "</th>");
        }
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr>");
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                sb.Append("<td>" + dt.Rows[i][j] + "</td>");
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
        sb.Append("</table>");
        return sb.ToString();
    }

    public static string ErrorToDatatableStr(string FieldName, string ErrorMsg)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table class='table table-striped table-bordered table-sm'>");
        sb.Append("<thead>");
        sb.Append("<tr>");
        sb.Append("<th>Field Name</th>");
        sb.Append("<th>Error Description</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");
        sb.Append("<tr>");
        sb.Append("<td>" + FieldName + "</td>");
        sb.Append("<td>" + ErrorMsg + "</td>");
        sb.Append("</tr>");
        sb.Append("</tbody>");
        sb.Append("</table>");
        return sb.ToString();
    }

    //-------- Not In Use
    public static string CreateErrorRpt(DataSet DsError)
    {
        string[] ArrColor = new string[14];
        ArrColor[0] = "#ee88a4";
        ArrColor[1] = "#55c2ff";
        ArrColor[2] = "#a1b49b";
        ArrColor[3] = "#00ff7f";
        ArrColor[4] = "#bdac73";
        ArrColor[5] = "#f5d45e";
        ArrColor[6] = "#ce05f0";
        ArrColor[7] = "#c0d6e4";
        ArrColor[8] = "#f08080";
        ArrColor[9] = "#ffc3a0";
        ArrColor[10] = "#6897bb";
        ArrColor[11] = "#81d8d0";
        ArrColor[12] = "#e18a7a";
        ArrColor[13] = "#967b59";

        DataTable dt = DsError.Tables[0];
        DataTable dtError = DsError.Tables[1];
        StringBuilder sb = new StringBuilder();
        sb.Append("<table class='clsErrorList'>");
        sb.Append("<tbody>");
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            sb.Append("<tr>");
            sb.Append("<td><div class='clsColor-block' style='background:" + ArrColor[Convert.ToInt32(dt.Rows[i]["ErrorId"]) - 1] + "'></div></td><td>" + dt.Rows[i]["ErrorDescr"] + "</td>");
            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
        sb.Append("</table>");

        sb.Append("<div id='divErrortbl' style='overflow-y: auto;'>");
        sb.Append("<table class='clsErrortbl table table-striped table-bordered table-sm'>");
        sb.Append("<thead>");
        sb.Append("<tr>");
        for (int j = 0; j < dtError.Columns.Count; j++)
        {
            sb.Append("<th>" + dtError.Columns[j].ColumnName.ToString() + "</th>");
        }
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");
        for (int i = 0; i < dtError.Rows.Count; i++)
        {
            sb.Append("<tr>");
            for (int j = 0; j < dtError.Columns.Count; j++)
            {
                if (dtError.Rows[i][j].ToString().Split('^').Length > 1)
                {
                    sb.Append("<td style='background:" + ArrColor[Convert.ToInt32(dtError.Rows[i][j].ToString().Split('^')[1]) - 1] + "'>" + dtError.Rows[i][j].ToString().Split('^')[0] + "</td>");
                }
                else
                {
                    sb.Append("<td>" + dtError.Rows[i][j] + "</td>");
                }
            }
            sb.Append("</tr>");
        }
        sb.Append("</tbody>");
        sb.Append("</table>");
        sb.Append("</div>");
        return sb.ToString();
    }


    

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}