Imports System.Data
Imports System.Data.SqlClient
Imports System.Text

Partial Class Site
    Inherits System.Web.UI.MasterPage
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsPostBack Then
            'fnGetMenuHierarchy()
            hdnRoleID.Value = Convert.ToString(Session("RoleID"))
            ' If Session("clsMenuHTML") Is Nothing Then
            dropdown.InnerHtml = ClsMenuItem.PopulateProductTree(Session("LoginID").ToString())
            ' DvMenu.InnerHtml = Session("clsMenuHTML").ToString()
            'Else
            'DvMenu.InnerHtml = Session("clsMenuHTML").ToString()
            'End If

        End If
    End Sub


    Protected Sub lnkLogout_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/frmSessionTimeOut.aspx?flgcallfrom=4")
    End Sub
End Class

