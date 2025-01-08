<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="ChannelLockingPeriodExt.aspx.cs" Inherits="ChannelLockingPeriodExt" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script type="text/javascript">
        function fnRefreshPage() {
            fnShowReport();
        }

        function fnFailed(result) {
            alert("Error : " + result);
            $("#divloader").hide();
        }

        $(document).ready(function () {
            $("#divRptTitle").html("Channel Locking Period");
            $("#divContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 15) + "px");

            fnShowReport();
        });

        function fnShowReport() {
            $("#divloader").show();
            var LoginId = $("#ConatntMatter_hdnLoginId").val();
            var UserId = $("#ConatntMatter_hdnUserId").val();

            PageMethods.GetReport(LoginId, UserId, GetReport_pass, fnFailed);
        }
        function GetReport_pass(result) {
            $("#divloader").hide();

            if (result.split("|^|")[0] == "0") {
                $("#divReport").html(result.split("|^|")[1]);

                var wid = $("#tblDateExt").width();
                var thead = $("#tblDateExt").find("thead").eq(0).html();
                $("#divHeader").html("<table id='tblDateExt_header' class='table table-bordered table-sm clsDateExt' style='margin-top:-4px; margin-bottom:0; width:" + (wid - 1) + "px; min-width:" + (wid - 1) + "px;'><thead>" + thead + "</thead></table>");
                $("#tblDateExt").css("width", wid);

                $("#tblDateExt").css("min-width", wid);
                for (i = 0; i < $("#tblDateExt").find("th").length - 1; i++) {
                    var th_wid = $("#tblDateExt").find("th")[i].clientWidth;
                    $("#tblDateExt_header").find("th").eq(i).css("min-width", th_wid);
                    $("#tblDateExt_header").find("th").eq(i).css("width", th_wid);
                    $("#tblDateExt").find("th").eq(i).css("min-width", th_wid);
                    $("#tblDateExt").find("th").eq(i).css("width", th_wid);
                }
                $("#tblDateExt").css("margin-top", "-" + $("#tblDateExt_header")[0].offsetHeight + "px");

                $("#divReport").height($(window).height() - ($("div.card-header").height() + $("#tblDateExt_header")[0].offsetHeight + 120) + "px");

                $(".cls-date").datepicker({
                    dateFormat: 'dd-M-yy'
                    //minDate: 0
                });
            }
            else {
                $("#divHeader").html("");
                $("#divReport").html("Error : " + result.split("|^|")[1]);
            }
        }

        function fnExtendDate(ctrl) {
            var LoginId = $("#ConatntMatter_hdnLoginId").val();
            var YearVal = $(ctrl).closest("tr").attr("YearVal");
            var MonthVal = $(ctrl).closest("tr").attr("MonthVal");
            var MonthName = $(ctrl).closest("tr").find("td").eq(1).html();
            var strDate = $(ctrl).closest("tr").find("input[iden='lockingDate']").eq(0).val();

            if (strDate == "") {
                alert("Please select the Locking Date for the Month - " + MonthName + " !");
            }
            else {
                $("#divloader").show();

                PageMethods.fnSave(MonthVal, YearVal, strDate, LoginId, fnSave_pass, fnFailed, MonthName);
            }
        }
        function fnSave_pass(res, MonthName) {
            if (res.split("|^|")[0] == "0") {
                alert("Date Extended successfully for the Month - " + MonthName + " !");
                fnShowReport();
            }
            else {
                fnFailed(res.split("|^|")[1]);
            }
        }


        // ------------------------------ Not In Use -------------------------------------
        function fnAdd() {
            var str = "<tr flgEdit='1' UserID='0'>";
            str += "<td></td>";
            str += "<td><input type='text' class='form-control form-control-sm'/></td>";
            str += "<td><input type='text' class='form-control form-control-sm'/></td>";
            str += "<td><input type='checkbox' checked/></td>";
            str += "<td><a href='#' class='btn btn-primary btn-xs' selectedstr='' onclick='fnBranchPopup(this);'/>Click to select</a></td>";
            str += "<td data-title='actionbtn'><img title='Save' src='../../Images/save_blue.png' onclick='fnIndividualSave(this);'/><img title='Cancel' src='../../Images/cancel_blue.png' class='ml-2' onclick='fnCancel(this);'/></td>";
            str += "</tr>";
            $("#tblDateExt").find("tbody").eq(0).prepend(str);
        }

    </script>

    <style type="text/css">
        input[type='text'] {
            width: 100%;
        }

        a.btn {
            font-size: 0.7rem;
            padding: 0.3rem 0.5rem;
        }

        input[type='text'].form-control {
            font-size: 0.76rem;
        }

        .main-content {
            padding-bottom: 8px;
        }
    </style>
    <style type="text/css">
        #divReport {
            overflow-y: auto;
            overflow-x: hidden;
        }

        table.clsDateExt {
            width: 75%;
            margin: 0 0 0 12%;
        }

            table.clsDateExt > thead > tr > th {
                color: #093D6D;
                font-size: 0.8rem;
                background: #f5f8ff;
                text-align: center;
                vertical-align: middle;
                text-transform: uppercase;
                border-bottom: 2px solid #006092 !important;
            }

            table.clsDateExt tr th:nth-child(1) {
                width: 10%;
            }

            table.clsDateExt tr th:nth-child(2),
            table.clsDateExt tr th:nth-child(3) {
                width: 35%;
            }


            table.clsDateExt td {
                font-size: 0.9rem;
                vertical-align: middle;
            }

            table.clsDateExt tr td:nth-child(1),
            table.clsDateExt tr td:nth-child(4) {
                text-align: center;
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
        <div id="divContainer">
            <div style="text-align: right; padding-right: 10px; display: none;">
                <input type='button' class='btns btn-submit mt-2' style="padding: 0 20px;" value='Add New User' onclick='fnAdd()' />
            </div>
            <div class="w-50 m-auto">
                <div class="card-body pb-0" id="divHeader"></div>
                <div class="card-body pt-0" id="divReport"></div>
            </div>
        </div>
    </div>

    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>

    <div id="divBranchHier" style="display: none;"></div>

    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />

    <asp:HiddenField ID="hdnBranchHierMstr" runat="server" />
</asp:Content>

