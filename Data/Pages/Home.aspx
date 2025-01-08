<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />

    <script type="text/javascript">        
        $(document).ready(function () {
            $("#divRptTitle").html("welcome " + $("#ConatntMatter_hdnUserName").val().toString().toUpperCase());

            $("#divContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 15) + "px");
        });

        function fnSniper() {
            window.location.href = "Dashboard.aspx?id=0";
        }
        function fnHawkeye() {
            window.location.href = "Dashboard.aspx?id=20";
        }
        function MaximusCommandCenter() {
            window.location.href = "Dashboard.aspx?id=28";
        }
        function fnChannelUpdate() {
            window.location.href = "ChannelUpdate.aspx?id=30";
        }

        function fnMenu(ctrl) {
            var MenuID = $(ctrl).attr("Menuflg");
            switch (MenuID.toString()) {
                case "8":
                    window.location.href = "SniperHome.aspx?id=0";
                    //window.location.href = "Dashboard.aspx?id=0";
                    break;
                case "19":
                    window.location.href = "Dashboard.aspx?id=20";
                    break;
                case "27":
                    window.location.href = "Dashboard.aspx?id=28";
                    break;
                case "29":
                    window.location.href = "ChannelUpdate.aspx?id=30";
                    break;
                case "34":
                    window.location.href = "Dashboard.aspx?id=34";
                    break;
                case "36":
                    window.location.href = "SBFGroupMapping.aspx?id=36";
                    break;
            }
        }
    </script>
    <style type="text/css">
        .main-content {
            padding-bottom: 0 !important;
        }

        .btn-submit {
            min-width: 250px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="card default_bg no_radius m_row pb-0">
        <div class="card-header green_bg pr-0">
            <div class="card-title-heading white_txt" id="divRptTitle" style="display: inline-block; width: 100%; text-align: center;"></div>
            <div class="card-title-actions blue_bg d-none" style="cursor: pointer">
                <img src="../../Images/refresh_icon.svg" onclick="fnRefreshPage()" class="icon" />
            </div>
        </div>
        <div class="text-center" id="divContainer">
            <div id="divbtns" runat="server" style="margin-top: 15%;">

            </div>
            <%--<div style="margin-top: 10%; box-sizing: border-box;">
                <input type='button' class='btns btn-submit' title="Click on this Tab" value='Red Flag Command Center' onclick='fnSniper()' />
                <input type='button' class='btns btn-submit ml-3' title="Click on this Tab" value='Sub-D Command Center' onclick='fnHawkeye()' />
                <input type='button' class='btns btn-submit ml-3' title="Click on this Tab" value='Maximus Command Center' onclick='MaximusCommandCenter()' />
            </div>
            <div class="m-4" style="margin-top: 10%; box-sizing: border-box;">
                <input type='button' class='btns btn-submit' style="min-width: 250px;" title="Click on this Tab" value='Channel Update' onclick='fnChannelUpdate()' />
            </div>--%>
        </div>
    </div>
    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
</asp:Content>
