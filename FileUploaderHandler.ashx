<%@ WebHandler Language="C#" Class="FileUploaderHandler" %>

using System;
using System.Web;
using System.IO;

public class FileUploaderHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            var postedFile = context.Request.Files;
            string UserID = context.Request.Form["UserID"].ToString();
            string NodeID = context.Request.Form["NodeID"].ToString();
            string LoginID = context.Request.Form["LoginID"].ToString();
            string FileID = context.Request.Form["FileID"].ToString();
            string FileName = context.Request.Form["FileName"].ToString();
            string Actionflg = context.Request.Form["Actionflg"].ToString();

            if (Actionflg == "1")
            {
                if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/JBP")))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/JBP"));

                if (!File.Exists(HttpContext.Current.Server.MapPath("~/Files/JBP/Temp")))
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/Files/JBP/Temp"));

                string NewFileName = "JBP_" + FileID + "_" + NodeID + "_" + UserID + "_" + LoginID + "_" + DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss") + Path.GetExtension(postedFile[0].FileName);
                postedFile[0].SaveAs(HttpContext.Current.Server.MapPath("~/Files/JBP/Temp/") + NewFileName);

                context.Response.Write("0^" + NewFileName);
            }
            else if (Actionflg == "9")
            {
                string FilePath = HttpContext.Current.Server.MapPath("~/Files/JBP/Temp/") + FileName;
                File.Copy(FilePath, HttpContext.Current.Server.MapPath("~/Files/JBP/") + FileName, true);

                if (File.Exists(FilePath))
                    File.Delete(FilePath);

                context.Response.Write("0^" + FileName);
            }
        }
        catch (Exception ex)
        {
            context.Response.Write("1^Error : " + ex.Message);
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}