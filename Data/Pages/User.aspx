<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="User.aspx.cs" Inherits="Data_Dashboard_Dashboard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />

    <script type="text/javascript">
        function fnRefreshPage() {
            fnShowReport();
        }

        $(document).ready(function () {
            fnShowReport();
        });


        function fnShowReport() {
            $("#dvFadeForProcessing").show();
            var LoginId = $("#ConatntMatter_hdnLoginId").val();

            PageMethods.GetUserList(LoginId, GetUserList_pass, fnFailed);
        }

        function GetUserList_pass(result) {
            $("#dvFadeForProcessing").hide();
            $("#divUser").html(result);

            $("#divUser").height($(window).height() - ($("div.card-header").height() + 160) + "px");
        }

        function fnFailed(result) {
            alert("Error : " + result.get_message());
            $("#dvFadeForProcessing").hide();
        }

        function fnAdd() {
            alert(1);
        }

        function fnEdit(ctrl) {
            alert(2);
        }

    </script>
    <style type="text/css">
        #divUser {
            overflow-y: auto;
            overflow-x: hidden;
            padding-top: 1rem;
        }

        table.clsUser {
            width: 74%;
            margin: 0 auto;
        }

            table.clsUser > thead > tr > th {
                color: #093D6D;
                background: #f5f8ff;
                text-align: center;
                vertical-align: middle;
                text-transform: uppercase;
                border-bottom: 2px solid #006092 !important;
            }

            table.clsUser tr th:nth-child(3),
            table.clsUser tr th:nth-child(5) {
                width: 80px;
            }


            table.clsUser td {
                font-size: 0.8rem;
            }

            table.clsUser tr td:nth-child(3),
            table.clsUser tr td:nth-child(4),
            table.clsUser tr td:nth-child(5) {
                text-align: center;
            }

            table.clsUser img {
                cursor: pointer;
            }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ConatntMatter" runat="Server">
    <div class="card default_bg no_radius m_row">
        <div class="card-header green_bg pr-0">            
            <div class="card-title-heading white_txt" id="divRptTitle" style="display: inline-block; width: 100%; text-align: center;"></div>
            <div class="card-title-actions blue_bg" style="cursor: pointer">
                <img src="../../Images/refresh_icon.svg" onclick="fnRefreshPage()" class="icon" />
            </div>
        </div>
        <div style="text-align:right; padding-right: 10px;">
            <input type='button' class='btns btn-submit mt-2' style="padding: 0 20px;" value='Add New User' onclick='fnAdd()' />
        </div>
        <div class="card-body" id="divUser">
        </div>
    </div>

    <div id="dvFadeForProcessing" class="loader_bg" style="display: none">
        <div class="loader"></div>
    </div>
    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
</asp:Content>

