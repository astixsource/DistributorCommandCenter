<%@ WebHandler Language="C#" Class="ExcelReadAndBulkCopyUpload" %>

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
using Newtonsoft.Json;
using ExcelDataReader;

public class ExcelReadAndBulkCopyUpload : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var postedFile = context.Request.Files[0];
        string FileName = Path.GetFileName(postedFile.FileName);
        string LoginId = context.Request.Form["LoginId"].ToString();
        string MonthYr = context.Request.Form["MonthYr"].ToString();
        string FileType = context.Request.Form["FileType"].ToString();
        try
        {
            string msg = UploadFile(postedFile, MonthYr, LoginId, FileType, context);
            context.Response.Write(msg);
        }
        catch (Exception ex)
        {
            //SendMail(ConfigurationManager.AppSettings["MailTO"].ToString(), "PNG File Upload : Error while Uploading File", "Error while Uploading File :<br/>File Type : " + FileType + "<br/>File Name : " + Path.GetFileName(postedFile.FileName) + "<br/>Date-Time : " + DateTime.Now.ToString() + "<br/>Error : " + ex.Message, "");

            context.Response.Write("1^Error : " + ex.Message + " !");
        }
    }


    private string UploadFile(HttpPostedFile File_Up, string MonthYr, string LoginId, string FileType, HttpContext context)
    {
        try
        {
            string FolderName = "";
            switch (FileType)
            {
                case "1":
                    FolderName = "SubD";
                    break;
                case "2":
                    FolderName = "WS";
                    break;
            }

            //Save the uploaded file at new location.
            //string filename = Path.GetFileNameWithoutExtension(File_Up.FileName) + "_" + FileSetID + "_" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + Path.GetExtension(File_Up.FileName);

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/" + FolderName)))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/" + FolderName));

            if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/" + FolderName + "/Temp")))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/" + FolderName + "/Temp"));

            string NewFileName = FolderName + "_" + MonthYr + "_" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + Path.GetExtension(File_Up.FileName);
            string filePath = HttpContext.Current.Server.MapPath("~/Files/" + FolderName + "/Temp/") + NewFileName;
            File_Up.SaveAs(filePath);


            DataSet DsExcelData = new DataSet();
            using (FileStream oStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader iExcelDataReader = null;
                string extension = Path.GetExtension(filePath);
                var conf = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true
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

            if (DsExcelData.Tables.Count == 0)
                return "1^No Worksheet found !";

            if (DsExcelData.Tables[0].Rows.Count == 0)
            {
                DsExcelData.Dispose();
                return "1^Error : No data found in selected File.";
            }

            string IsValidTarget = ValidateTagetField(DsExcelData.Tables[0], FileType);
            if (IsValidTarget != "0")
            {
                DsExcelData.Dispose();
                return "1^Error : " + IsValidTarget;
            }

            DataTable dt = DsExcelData.Tables[0].Copy();
            dt.Columns.Add("LoginID", typeof(Int32));
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["LoginID"] = LoginId;

            dt.AcceptChanges();

            string strUpload = UploadData(dt, FileType);
            DsExcelData.Dispose();

            if (strUpload == "0")
            {
                SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
                SqlCommand Scmd = new SqlCommand();
                Scmd.Connection = Scon;

                switch (FileType)
                {
                    case "1":
                        Scmd.CommandText = "[spSubdHawkeyeLoadSubDTarget]";
                        break;
                    case "2":
                        Scmd.CommandText = "[spSubdHawkeyeLoadSubDWSTarget]";
                        break;
                }

                Scmd.Parameters.AddWithValue("@LoginID", HttpContext.Current.Session["LoginID"].ToString());
                Scmd.CommandType = CommandType.StoredProcedure;
                Scmd.CommandTimeout = 0;
                SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
                DataSet Ds = new DataSet();
                Sdap.Fill(Ds);

                File.Copy(filePath, HttpContext.Current.Server.MapPath("~/Files/" + FolderName + "/") + NewFileName, true);

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            else
                return strUpload;

            return "0^File Uploaded successfully at " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + ".";
        }
        catch (Exception ex)
        {
            //SendMail(ConfigurationManager.AppSettings["MailTO"].ToString(), "PNG File Upload : Error while Uploading File", "Error while Uploading File :<br/>File Type : " + FileType + "<br/>File Name : " + Path.GetFileName(File_Up.FileName) + "<br/>Date-Time : " + DateTime.Now.ToString() + "<br/>Error : " + ex.Message, "");

            return "1^Due to some technical reasons, we are unable to process your request. Error : " + ex.Message + " !";
        }
    }

    public string ValidateTagetField(DataTable dt, string FileType)
    {
        decimal d = 0;
        int flgValid = 0;
        string msg = "";
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["Target"].ToString() != "")
            {
                if (!(decimal.TryParse(dt.Rows[i]["Target"].ToString(), out d)))
                {
                    flgValid = 1;
                    msg = "Taget value at Row No. " + (i + 2) + " is not valid.";
                    break;
                }
            }
        }

        if (flgValid == 0)
            return "0";
        else
            return msg;
    }

    public string UploadData(DataTable dtRecords, string FileType)
    {
        string[] ArrColumnName = new string[0];
        switch (FileType)
        {
            case "1":           // SubD Target
                ArrColumnName = new string[6];
                ArrColumnName[0] = "SubDCode";
                ArrColumnName[1] = "SubDName";
                ArrColumnName[2] = "Month";
                ArrColumnName[3] = "Year";
                ArrColumnName[4] = "Target";
                ArrColumnName[5] = "LoginID";
                break;
            case "2":           // WS Target
                ArrColumnName = new string[8];
                ArrColumnName[0] = "SubdCode";
                ArrColumnName[1] = "SubDName";
                ArrColumnName[2] = "StoreCode";
                ArrColumnName[3] = "StoreName";
                ArrColumnName[4] = "Month";
                ArrColumnName[5] = "Year";
                ArrColumnName[6] = "Target";
                ArrColumnName[7] = "LoginID";
                break;
        }

        SqlConnection Scon = new SqlConnection(ConfigurationManager.AppSettings["strConn"]);
        try
        {
            if (dtRecords.Columns.Count != ArrColumnName.Length)
                return "1^Column count mis-match. Plz re-verify the File !";

            for (int j = 0; j < dtRecords.Columns.Count; j++)
            {
                if (!ArrColumnName.Contains(dtRecords.Columns[j].ColumnName.ToString().Trim()))
                    return "1^" + dtRecords.Columns[j].ColumnName.ToString().Trim() + " column is not a valid defined column. Please check..";
            }

            Scon.Open();
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Scon))
            {
                bulkCopy.BatchSize = 1000;
                bulkCopy.NotifyAfter = 1000;

                switch (FileType)
                {
                    case "1":
                        bulkCopy.DestinationTableName = "tmpSubdHawkeyeSubDTarget";
                        bulkCopy.ColumnMappings.Add(ArrColumnName[0], "[SubDCode]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[1], "[SubDName]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[2], "[Month]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[3], "[Year]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[4], "[Target]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[5], "[LoginID]");
                        break;
                    case "2":
                        bulkCopy.DestinationTableName = "tmpSubdHawkeyeSubDWSTarget";
                        bulkCopy.ColumnMappings.Add(ArrColumnName[0], "[SubDCode]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[1], "[SubDName]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[2], "[Storecode]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[3], "[StoreName]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[4], "[Month]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[5], "[Year]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[6], "[Target]");
                        bulkCopy.ColumnMappings.Add(ArrColumnName[7], "[LoginID]");
                        break;
                }

                System.Data.SqlClient.SqlBulkCopyColumnMappingCollection sbcmc = bulkCopy.ColumnMappings;
                bulkCopy.WriteToServer(dtRecords);
            }

            return "0";
        }
        catch (Exception ex)
        {
            //SendMail(ConfigurationManager.AppSettings["MailTO"].ToString(), "PNG File Upload : Error while Uploading File", "Error while Uploading File :<br/>File Type : " + FileType + "<br/>FileSetID : " + dtRecords.Rows[0]["FileSetID"].ToString() + "<br/>Date-Time : " + DateTime.Now.ToString() + "<br/>Error : " + ex.Message, "");

            return "1^Due to some technical reasons, we are unable to process your request. Error : " + ex.Message + " !";
        }
        finally
        {
            Scon.Close();
            Scon.Dispose();
        }
    }

    public static string DatatabletoStr(DataTable dt)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<table class='table table-striped table-bordered table-sm'>");
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

    

    static bool validatefiledate(string date)
    {
        bool isValid = false;
        try
        {

            Regex regex = new Regex(@"^\d{4}((0\d)|(1[012]))(([012]\d)|3[01])$");///([12]\d{3}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01]))/
            isValid = regex.IsMatch(date.Trim());
        }
        catch (Exception ex)
        {
            isValid = false;
        }

        return isValid;
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}