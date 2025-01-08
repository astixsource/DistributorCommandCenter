using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Write(Uri.EscapeDataString("JV501 GST On' Transport + Recovery PN2E# Dec  2021_Reprocess_3460_108_31-01-2022-03-51-06.xlsx").Replace("'", "%27").Replace(@"""", "%22"));//
    }
}