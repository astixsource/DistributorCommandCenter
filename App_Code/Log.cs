using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Log
/// </summary>
public static class Log
{
    public static void CreateLog(string FolderName, string FlgType, string Status, string ToMail, string Msg)
    {
        string strDirectoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\";
        string strFilePath = strDirectoryPath + FolderName + "\\" + System.DateTime.Now.Year.ToString("0000") + "-" + System.DateTime.Now.Month.ToString("00") + "-" + System.DateTime.Now.Day.ToString("00") + ".log";
        StreamWriter strWrite = default(StreamWriter);
        try
        {
            if (!System.IO.Directory.Exists(strDirectoryPath + FolderName + "\\"))
                System.IO.Directory.CreateDirectory(strDirectoryPath + FolderName + "\\");

            if (System.IO.File.Exists(strFilePath))
                strWrite = File.AppendText(strFilePath);
            else
                strWrite = File.CreateText(strFilePath);

            strWrite.WriteLine("Type        : " + FlgType);
            strWrite.WriteLine("Status      : " + Status);
            strWrite.WriteLine("Email       : " + ToMail);
            strWrite.WriteLine("Date & Time : " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            strWrite.WriteLine("Message     : " + Msg);
            strWrite.WriteLine("  --------------------------------------------------------------------------------------  ");
            strWrite.WriteLine("   ");
            strWrite.Close();
        }
        catch
        {
            //
        }
        finally
        {
            strWrite = null;
            System.GC.Collect();
        }
    }
}