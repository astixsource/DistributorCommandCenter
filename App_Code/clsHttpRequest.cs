using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Web;
using System.IO;

public class clsHttpRequest
{

    public static HttpWebResponse POST(string url)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = 0;

            return (HttpWebResponse)request.GetResponse();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static string SendNotification(string token, string Msg, string flag, string MobNo, string StoreId, string StoreName)
    {
        try
        {
            string SERVER_API_KEY = ConfigurationManager.AppSettings["Notification_SERVER_API_KEY"];
            string SENDER_ID = ConfigurationManager.AppSettings["Notification_SENDER_ID"];

            var data = new
            {
                data = new
                {
                    message = Msg,
                    flag = flag,
                    status = true,
                    MobNo = MobNo,
                    storeId = StoreId,
                    storeName = StoreName
                },
                to = token,
                priority = "high"
            };

            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(data);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            WebRequest req = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            req.Method = "post";
            req.ContentType = "application/json";
            req.Headers.Add(String.Format("Authorization: key={0}", SERVER_API_KEY));
            req.Headers.Add(String.Format("Sender: id={0}", SENDER_ID));

            req.ContentLength = byteArray.Length;
            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse res = req.GetResponse();
            dataStream = res.GetResponseStream();
            StreamReader read = new StreamReader(dataStream);

            string ResponsefrmServer = read.ReadToEnd();

            read.Close();
            dataStream.Close();
            res.Close();
            FnWriteLogFile_Log(token, ResponsefrmServer);
            return "success";
        }
        catch (Exception ex)
        {
            FnWriteLogFile_Log(token, ex.Message);
            return "failure";
        }
    }
    public static void FnWriteLogFile_Log(string sToten, string message)
    {
        string LogPath_Log = HttpContext.Current.Server.MapPath("~/Logs/SendNotification_") + HttpContext.Current.Session["username"].ToString() + "_" + DateTime.Now.ToString("yyyy_MM_dd") + "_log.txt"; // This is Log File Path Where we Generate Log File

        using (var sw = new StreamWriter(LogPath_Log, true))
        {
            sw.WriteLine();
            sw.WriteLine("Date Time :" + DateTime.Now);
            sw.WriteLine(message);
            sw.WriteLine(sToten);
        }

    }
    
}
