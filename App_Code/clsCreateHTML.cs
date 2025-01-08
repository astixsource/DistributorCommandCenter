using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// Summary description for clsCreateHTML
/// </summary>
public class clsCreateHTML
{
    public static string createSubTbl(DataTable dt, string[] SkipColumn, string strpadding, int flgExcel)
    {
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            str.Append("<tr style='" + (flgExcel == 0 ? "display:none" : "") + "' lvl='1' nodeid='" + dt.Rows[i]["NodeId"] + "' nodetype='" + dt.Rows[i]["NodeType"] + "' pnodeid='" + dt.Rows[i]["PNodeId"] + "' pnodetype='" + dt.Rows[i]["PNodeType"] + "'>");
            int flgpadd = 0;
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string sColumnName = dt.Columns[j].ColumnName;
                if (SkipColumn.Contains(sColumnName))
                {
                    continue;
                }
                var sdata = dt.Rows[i][j];
                string sData = "";
                sData = dt.Rows[i][j].ToString();
                if (flgpadd == 0)
                {
                    str.Append("<td style='text-align:left;'>" + sData + "</td>");
                }
                else
                {
                    str.Append("<td style='text-align:right;'>" + sData + "</td>");
                }
                flgpadd = 1;
            }
                str.Append("<td style='text-align:right;'><a href='###' onclick='fnDownloadSiteDetails(this)' title='Click to download report'><span class='glyphicon glyphicon-download'></span></td>");
            str.Append("</tr>");
        }
        return str.ToString();
    }
    public static string createtbl_Measures(DataTable dt, string[] SkipColumn, string str)
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("<div class='card default_bg mt-2'>");
        sb.Append("<div class='card-header'>");
        sb.Append("<div class='card-title-heading default_txt'>" + str + "");
        //sb.Append("<div class='page-title-subheading' id='divHeader0class' runat='server'>Sub : Manual Journal Request Format</div>");
        sb.Append("</div>");


        //sb.Append("<div class='widget-content-wrapper text-white'>");

        //sb.Append("<div class='widget-content-left'>");
        //sb.Append("<div class='widget-heading'>" + str + "</div>");
        //sb.Append("<div class='widget-subheading' id='divHeader0class' runat='server'>Sub : Manual Journal Request Format</div>");
        //sb.Append("</div>");
        //sb.Append("</div>");

        if (Convert.ToString(dt.Rows[0]["Comments"]) != "")
        {
            sb.Append("<div class='card-right-actions-pane'><a href='###' onclick='fnShowSupportingFilesDialog(this)'><i class='fa fa-download'></i> &nbsp;Download Supporting</a><a href='###' class='ml-2' onclick='fnShowComments(this)' scomments='" + HttpContext.Current.Server.HtmlEncode(Convert.ToString(dt.Rows[0]["Comments"])) + "'><i class='fa fa-comment-o'></i> &nbsp;Show Comments</a></div>");
        }
        else
        {
            sb.Append("<div class='card-right-actions-pane'><a href='###' onclick='fnShowSupportingFilesDialog(this)'><i class='fa fa-download'></i> Download Supporting</a></div>");
        }
        sb.Append("</div>");
        sb.Append("<div class='card-body pt-1 pb-1'><div class='table-responsive'>");
        sb.Append("<table id='tblMeasure' class='table table-sm table-bordered mb-0'>");
        //sb.Append("<tr>");
        //sb.Append("<td class='clsheader1Class' colspan='3'>" + str+"</td>");
        //if (Convert.ToString(dt.Rows[0]["Comments"]) != "")
        //{
        //    sb.Append("<td colspan='3'><a href='../../UploadedFiles/" + Convert.ToString(dt.Rows[0]["FileName"]) + "' class='fa fa-download' style='font-size:13pt;padding-left:20px' target='_blank' > Download Supporting</a></td><td colspan='2' style='font-size:13pt;text-align:right'><a href='###' scomments='"+HttpContext.Current.Server.HtmlEncode(Convert.ToString(dt.Rows[0]["Comments"])) + "' onclick='fnShowComments(this)' class='fa fa-comment-o '> Show Comments</a></td>");
        //}
        //else
        //{
        //    sb.Append("<td colspan='5'><a href='../../UploadedFiles/" + Convert.ToString(dt.Rows[0]["FileName"]) + "' class='fa fa-download' style='font-size:13pt;padding-left:20px' target='_blank' > Download Supporting</a></td>");
        //}
        //sb.Append("</tr>");
        int colCount = 0;
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                string sColumnName = dt.Columns[j].ColumnName;
                if (SkipColumn.Contains(sColumnName))
                {
                    continue;
                }
                if (colCount == 0)
                {
                    sb.Append("<tr>");
                }
                sb.Append("<th class='clsColClass'>"+ dt.Columns[j].ColumnName + "</th>");
                if(dt.Columns[j].ColumnName=="Total JV Value" || dt.Columns[j].ColumnName == "Posting Month" || dt.Columns[j].ColumnName == "Reversal Month" || dt.Columns[j].ColumnName.ToLower() == "1st approver" || dt.Columns[j].ColumnName.ToLower() == "2nd approver" || dt.Columns[j].ColumnName== "Debit = Credit")
                {
                    sb.Append("<td  class='clsCellClass clsbg'>" + Convert.ToString(dt.Rows[0][j]) + "</td>");
                }
                else
                {
                    sb.Append("<td  class='clsCellClass'>" + Convert.ToString(dt.Rows[0][j]) + "</td>");
                }
                colCount++;
                if (colCount == 4)
                {
                    colCount = 0;
                    sb.Append("</tr>");
                }
            }
        }
        if (colCount <4)
        {
            sb.Append("</tr>");
        }

       
        sb.Append("</table>");
        sb.Append("</div></div>");
        sb.Append("</div>");
        return sb.ToString();
    }
}