<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="UserMstr - Copy.aspx.cs" Inherits="UserMstr" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <script type="text/javascript">
        function fnRefreshPage() {
            window.location.href = window.location.href;
        }

        function fnFailed(result) {
            alert("Error : " + result);
            $("#divloader").hide();
        }

        $(document).ready(function () {
            $("#divRptTitle").html("User Master");

            alert(1);
            fnShowReport();
        });

        function fnShowReport() {
            $("#divloader").show();
            var LoginId = $("#ConatntMatter_hdnLoginId").val();
            alert(1);
            PageMethods.GetUserList(LoginId, GetUserList_pass, fnFailed);
        }
        function GetUserList_pass(result) {
            $("#divloader").hide();
            if (result.split("|^|")[0] == "0") {
                $("#divReport").html(result.split("|^|")[1]);

                var wid = $("#tblUser").width();
                var thead = $("#tblUser").find("thead").eq(0).html();
                $("#divHeader").html("<table id='tblUser_header' class='table table-bordered table-sm clsJBP' style='margin-top:-4px; margin-bottom:0; width:" + (wid - 1) + "px; min-width:" + (wid - 1) + "px;'><thead>" + thead + "</thead></table>");
                $("#tblUser").css("width", wid);

                $("#tblUser").css("min-width", wid);
                for (i = 0; i < $("#tblUser").find("th").length - 1; i++) {
                    var th_wid = $("#tblUser").find("th")[i].clientWidth;
                    $("#tblUser_header").find("th").eq(i).css("min-width", th_wid);
                    $("#tblUser_header").find("th").eq(i).css("width", th_wid);
                    $("#tblUser").find("th").eq(i).css("min-width", th_wid);
                    $("#tblUser").find("th").eq(i).css("width", th_wid);
                }
                $("#tblUser").css("margin-top", "-" + $("#tblUser_header")[0].offsetHeight + "px");

                $("#tblUser").find("tbody").eq(0).find("select").each(function () {
                    $(this).val($(this).closest("tr").attr("IsJBPDone"));
                });

                $("#divReport").height($(window).height() - ($("div.card-header").height() + $("#tblUser_header")[0].offsetHeight + 170) + "px");
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
        <div class="card-body pb-0" id="divHeader"></div>
        <div class="card-body pt-0" id="divReport">
        </div>
    </div>

    <div id="divloader" class="loader_bg" style="display: none">
        <div class="loader"></div>
    </div>
    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
</asp:Content>

