using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ClsMenuItem
/// </summary>
public class ClsMenuItem
{
	public ClsMenuItem()
	{
        
    }

    public static string PopulateProductTree(string loginValue)
    {
        SqlConnection Scon = new SqlConnection(System.Configuration.ConfigurationManager.AppSettings["strConnDCC"]);
        SqlCommand Scmd = new SqlCommand();
        Scmd.Connection = Scon;
        Scmd.CommandText = "spMakeTreeMenu";
        Scmd.CommandType = CommandType.StoredProcedure;
        Scmd.Parameters.AddWithValue("@MenuNode", "0");
        Scmd.Parameters.AddWithValue("@LoginID", loginValue);
        Scmd.CommandTimeout = 0;
        SqlDataAdapter Sdap = new SqlDataAdapter(Scmd);
        DataSet ds = new DataSet();
        Sdap.Fill(ds);
        Scmd.Dispose();
        Sdap.Dispose();

        //string[,] arrPara = new string[2, 2];
        //arrPara[0, 0] = "0";
        //arrPara[0, 1] = "0";
        //arrPara[1, 0] = loginValue;
        //arrPara[1, 1] = "0";

        //SqlConnection objCon = new SqlConnection();
        //SqlCommand objCom = new SqlCommand();
        //objCom.CommandTimeout = 0;

        //DataSet ds = new DataSet();
        //clsConnection.clsConnection objAdo = new clsConnection.clsConnection();
        //ds = objAdo.RunSPDS("spMakeTreeMenu", arrPara);

        ds.Relations.Add("rsParentChild", ds.Tables[0].Columns["HierID"], ds.Tables[0].Columns["PHierID"], false);
        int i = 0;
        //string lstlvl = ds.Tables[0].AsEnumerable().Max(tr => (Int32)tr["LstLevel"]).ToString();

        string strproduct = "";
        if (ds.Tables.Count > 0)
        {
            int marginleft = 10;
            strproduct += "<ul>";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["PHierID"].ToString() == "0" && dr["IndexNumP"].ToString() == "0")
                {
                    if (dr["IsLastLevel"].ToString() == "10")
                    {
                     strproduct += "<li class='has-submenu'><span id='" + dr["HierID"].ToString() + "^" + dr["IsLastLevel"].ToString() + "'>" + dr["Descr"].ToString() + "</span>";
                    }
                    else
                    {
                        if (dr["HierID"].ToString() == "8")
                        {
                            strproduct += "<li nid='" + dr["HierID"].ToString() + "' onclick=\"fnAction('" + dr["HierID"].ToString() + "')\" ><img src='../../Images/home_icon.svg' /><span id='" + dr["HierID"].ToString() + "^" + dr["IsLastLevel"].ToString() + "'>" + dr["Descr"].ToString() + "</span>";
                        }
                        else
                        {
                            strproduct += "<li nid='" + dr["HierID"].ToString() + "' onclick=\"fnAction('" + dr["HierID"].ToString() + "')\" ><span id='" + dr["HierID"].ToString() + "^" + dr["IsLastLevel"].ToString() + "'>" + dr["Descr"].ToString() + "</span>";
                        }
                    }
                    if (dr.GetChildRows("rsParentChild").Length > 0)
                    {
                        strproduct += PopulateProductChildTree(dr, marginleft);
                    }
                    strproduct += "</li>";
                }
                i = i + 1;
            }
            strproduct += "</ul>";
        }
        //objAdo.CloseConnection(ref objCon, ref objCom);

        return strproduct;
    }
    private static string PopulateProductChildTree(DataRow dr, int marginleft)
    {
        string strproduct = "";
        foreach (DataRow cRow in dr.GetChildRows("rsParentChild"))
        {
            if (cRow["IsLastLevel"].ToString() == "10")
            {
                strproduct += "<li class='has-submenu'><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span><div class='marker'></div>";
            }
            else
            {
                if (cRow["Descr"].ToString().Contains("Home"))
                {
                    strproduct += "<li nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/home_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }else if (cRow["Descr"].ToString().Contains("Process"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/inprogress_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("Pending"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/pending_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("History"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/history_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("Report"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/reports_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("Download"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/reports_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("Posting"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/managepost_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("SAP Doc"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/managepost_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("Management"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/manageuser_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else if (cRow["Descr"].ToString().Contains("Approver"))
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><img src='../../Images/managepost_icon.svg' /><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
                else
                {
                    strproduct += "<li   nid='" + cRow["HierID"].ToString() + "' onclick=\"fnAction('" + cRow["HierID"].ToString() + "')\" ><span id='" + cRow["HierID"].ToString() + "^" + cRow["IsLastLevel"].ToString() + "'>" + cRow["Descr"].ToString() + "</span>";
                }
            }
            if (cRow.GetChildRows("rsParentChild").Length > 0)
            {
                int marginleftt = marginleft + 8;
                strproduct += PopulateProductChildTree(cRow, marginleftt);
            }
            strproduct += "</li>";
        }
        return strproduct;
    }
}