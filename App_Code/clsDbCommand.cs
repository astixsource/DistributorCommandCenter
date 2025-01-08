using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
/// <summary>
/// Summary description for clsDbCommand
/// </summary>
public class clsDbCommand
{
    public clsDbCommand()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static string ExecuteQueryProcedure(string storedProcName, SqlConnection con,List<SqlParameter> parameters = null)
    {
        StringBuilder callDefinition = new StringBuilder();
        try
        {
            using (SqlCommand command = new SqlCommand(storedProcName, con))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                command.ExecuteNonQuery();
                return "1";
            }
        }
        catch (Exception ex)
        {
           
            try
            {
                string ProjectTitle = ConfigurationSettings.AppSettings["Title"];
                string ReferalUrl = System.IO.Path.GetFileName(HttpContext.Current.Request.UrlReferrer == null ? HttpContext.Current.Request.Url.AbsoluteUri : HttpContext.Current.Request.UrlReferrer.AbsolutePath);
                if (parameters != null)
                {
                    callDefinition.Append(string.Format("ExecuteStoredProc: {0} (", storedProcName));
                    int i = 0;
                    foreach (SqlParameter item in parameters)
                    {
                        string strOrderReturnDetail = JsonConvert.SerializeObject(item.Value, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        callDefinition.Append(string.Format("{0}={1}", item.ParameterName, strOrderReturnDetail));
                        if (i < parameters.Count() - 1)
                        {
                            callDefinition.Append("<br/>");
                        }
                        i++;
                    }
                    callDefinition.Append(")");
                }
                clsSendLogMail.fnSendLogMailLog(ex.Message, ex.ToString(), ReferalUrl, storedProcName, "Error in " + ReferalUrl + " in " + ProjectTitle, callDefinition.ToString());
            }
            catch (Exception e) { }
            throw;
        }
    }

    /// <summary>
    /// Executes a stored procedure with no return.
    /// </summary>
    /// <returns>The number of records affected by stored proc.</returns>
    public static string ExecuteQueryProcedure(string storedProcName, SqlConnection con, SqlTransaction transaction, List<SqlParameter> parameters = null)
    {
        StringBuilder callDefinition = new StringBuilder();
        try
        {
            using (SqlCommand command = new SqlCommand(storedProcName, con, transaction))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                command.ExecuteNonQuery();
                return "1";
            }
        }
        catch (Exception ex)
        {

            try
            {
                string ProjectTitle = ConfigurationSettings.AppSettings["Title"];
                string ReferalUrl = System.IO.Path.GetFileName(HttpContext.Current.Request.UrlReferrer == null ? HttpContext.Current.Request.Url.AbsoluteUri : HttpContext.Current.Request.UrlReferrer.AbsolutePath);
                if (parameters != null)
                {
                    callDefinition.Append(string.Format("ExecuteStoredProc: {0} (", storedProcName));
                    int i = 0;
                    foreach (SqlParameter item in parameters)
                    {
                        string strOrderReturnDetail = JsonConvert.SerializeObject(item.Value, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        callDefinition.Append(string.Format("{0}={1}", item.ParameterName, strOrderReturnDetail));
                        if (i < parameters.Count() - 1)
                        {
                            callDefinition.Append("<br/>");
                        }
                        i++;
                    }
                    callDefinition.Append(")");
                }
                clsSendLogMail.fnSendLogMailLog(ex.Message, ex.ToString(), ReferalUrl, storedProcName, "Error in " + ReferalUrl + " in " + ProjectTitle, callDefinition.ToString());
            }
            catch (Exception e) { }
            throw;
        }
    }

    /// <summary>
    /// Executes a query and returns a dataset
    /// </summary>
    public static DataSet ExecuteQueryReturnDataSet(string storedProcName, SqlConnection con, SqlTransaction transaction,List<SqlParameter> parameters = null)
    {
       StringBuilder callDefinition = new StringBuilder();
        try
        {
            

            using (SqlCommand command = new SqlCommand(storedProcName, con, transaction))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                    }
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        adapter.Fill(dataSet);
                        return dataSet;
                    }
                }
        }
        catch (Exception ex)
        {
            try
            {
                string ProjectTitle = ConfigurationSettings.AppSettings["Title"];
                string ReferalUrl = System.IO.Path.GetFileName(HttpContext.Current.Request.UrlReferrer == null ? HttpContext.Current.Request.Url.AbsoluteUri : HttpContext.Current.Request.UrlReferrer.AbsolutePath);
                if (parameters != null)
                {
                    callDefinition.Append(string.Format("ExecuteStoredProc: {0} (", storedProcName));
                    int i = 0;
                    foreach (SqlParameter item in parameters)
                    {
                        string strOrderReturnDetail = JsonConvert.SerializeObject(item.Value, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        callDefinition.Append(string.Format("{0}={1}", item.ParameterName, strOrderReturnDetail));
                        if (i < parameters.Count() - 1)
                        {
                            callDefinition.Append("<br/>");
                        }
                        i++;
                    }
                    callDefinition.Append(")");
                }
                clsSendLogMail.fnSendLogMailLog(ex.Message, ex.ToString(), ReferalUrl, storedProcName, "Error in " + ReferalUrl + " in " + ProjectTitle, callDefinition.ToString());
            }
            catch (Exception e) { }
            throw;
        }
    }


    /// <summary>
    /// Executes a query and returns a dataset
    /// </summary>
    public static DataSet ExecuteQueryReturnDataSet(string storedProcName, SqlConnection con,List<SqlParameter> parameters = null)
    {
        StringBuilder callDefinition = new StringBuilder();
        
        try
        {
            using (SqlCommand command = new SqlCommand(storedProcName, con))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters.ToArray());
                }
                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet);
                    return dataSet;
                }
            }
        }
        catch (Exception ex)
        {
            try
            {
                string ProjectTitle = ConfigurationSettings.AppSettings["Title"];
                string ReferalUrl = System.IO.Path.GetFileName(HttpContext.Current.Request.UrlReferrer == null ? HttpContext.Current.Request.Url.AbsoluteUri : HttpContext.Current.Request.UrlReferrer.AbsolutePath);
                if (parameters != null)
                {
                    callDefinition.Append(string.Format("ExecuteStoredProc: {0} (", storedProcName));
                    int i = 0;
                    foreach (SqlParameter item in parameters)
                    {
                        string strOrderReturnDetail = JsonConvert.SerializeObject(item.Value, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                        callDefinition.Append(string.Format("{0}={1}", item.ParameterName, strOrderReturnDetail));
                        if (i < parameters.Count() - 1)
                        {
                            callDefinition.Append("<br/>");
                        }
                        i++;
                    }
                    callDefinition.Append(")");
                }
                clsSendLogMail.fnSendLogMailLog(ex.Message, ex.ToString(), ReferalUrl, storedProcName, "Error in " + ReferalUrl + " in " + ProjectTitle, callDefinition.ToString());
            }
            catch (Exception e) { }
            throw;
        }
    }
}