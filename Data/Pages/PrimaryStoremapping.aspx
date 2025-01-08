<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="PrimaryStoremapping.aspx.cs" Inherits="PrimaryStoremapping" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />

    <script type="text/javascript">
        function fnRefreshPage() {
            fnShowReport();
        }

        function fnFailed(result) {
            alert("Error : " + result);
            $("#divloader").hide();
        }

        $(document).ready(function () {
            $("#divRptTitle").html("Primary Store Mapping");

            fnShowReport();
        });

        function fnShowReport() {
            $("#divloader").show();
            var LoginId = $("#ConatntMatter_hdnLoginId").val();
            var UserId = $("#ConatntMatter_hdnUserId").val();
            var NodeID = $("#ConatntMatter_hdnNodeID").val();

            PageMethods.GetReport(LoginId, UserId, NodeID, GetReport_pass, fnFailed);
        }
        function GetReport_pass(result) {
            $("#divloader").hide();

            if (result.split("|^|")[0] == "0") {
                $("#divReport").html(result.split("|^|")[1]);

                var wid = $("#tblMapping").width();
                var thead = $("#tblMapping").find("thead").eq(0).html();
                $("#divHeader").html("<table id='tblMapping_header' class='table table-bordered table-sm clsMapping' style='margin-top:-4px; margin-bottom:0; width:" + (wid - 1) + "px; min-width:" + (wid - 1) + "px;'><thead>" + thead + "</thead></table>");
                $("#tblMapping").css("width", wid);

                $("#tblMapping").css("min-width", wid);
                for (i = 0; i < $("#tblMapping").find("th").length-1; i++) {
                    var th_wid = $("#tblMapping").find("th")[i].clientWidth;
                    $("#tblMapping_header").find("th").eq(i).css("min-width", th_wid);
                    $("#tblMapping_header").find("th").eq(i).css("width", th_wid);
                    $("#tblMapping").find("th").eq(i).css("min-width", th_wid);
                    $("#tblMapping").find("th").eq(i).css("width", th_wid);
                }
                $("#tblMapping").css("margin-top", "-" + $("#tblMapping_header")[0].offsetHeight + "px");

                $("#tblMapping").find("tbody").eq(0).find("select[iden='branchcode']").each(function () {
                    $(this).html(result.split("|^|")[2]);
                    $(this).val($(this).closest("tr").attr("PriBranchCode"));
                });

                $("#tblMapping").find("tbody").eq(0).find("select[iden='branchname']").each(function () {
                    $(this).html(result.split("|^|")[3]);
                    $(this).val($(this).closest("tr").attr("PriBranchCode"));
                });

                $("#divReport").height($(window).height() - ($("div.card-header").height() + $("#tblMapping_header")[0].offsetHeight + 160) + "px");
            }
            else {
                $("#divHeader").html("");
                $("#divReport").html("Error : " + result.split("|^|")[1]);
            }
        }

        function fnSelectBranch(ctrl) {
            var code = $(ctrl).val();
            $(ctrl).closest("tr").find("select").val(code);
        }

        function fnEdit(ctrl) {
            $(ctrl).closest("tr").attr("flgEdit", "1");

            $(ctrl).closest("tr").find("select").removeAttr("disabled");
            $(ctrl).closest("tr").find("input[type='text']").removeAttr("disabled");
            $(ctrl).closest("td").eq(0).html("<img title='Save' src='../../Images/save_blue.png' onclick='fnIndividualSave(this);'/><img title='Cancel' src='../../Images/cancel_blue.png' class='ml-2' onclick='fnCancel(this);'/>");
        }
        function fnCancel(ctrl) {
            $(ctrl).closest("tr").attr("flgEdit", "0");

            $(ctrl).closest("tr").find("select").val($(ctrl).closest("tr").attr("PriBranchCode"));
            $(ctrl).closest("tr").find("select").prop("disabled", true);

            $(ctrl).closest("tr").find("input[type='text']").val($(ctrl).closest("tr").attr("LeapStoreCode"));
            $(ctrl).closest("tr").find("input[type='text']").prop("disabled", true);

            $(ctrl).closest("td").eq(0).html("<img src='../../Images/edit_blue.png' alt='edit' onclick='fnEdit(this)'/>");
        }
        function fnIndividualSave(ctrl) {
            var strId = $(ctrl).closest("tr").attr("strId");

            if ($(ctrl).closest("tr").find("select").eq(0).val() == "0") {
                alert("Please select the Branch !");
            }
            else if ($(ctrl).closest("tr").find("input[type='text']").eq(0).val() == "") {
                alert("Please enter the Leap Store Code !");
            }
            else {
                $("#divloader").show();
                var LoginId = $("#ConatntMatter_hdnLoginId").val();
                var UserId = $("#ConatntMatter_hdnUserId").val();
                var NodeID = $("#ConatntMatter_hdnNodeID").val();
                var strBranchCodeMapping = strId + "^" + $(ctrl).closest("tr").find("select").eq(0).val() + "|";
                var strBranchNameMapping = strId + "^" + $(ctrl).closest("tr").find("select").eq(1).find("option[value='" + $(ctrl).closest("tr").find("select").eq(0).val() + "']").eq(0).html() + "|";
                var strSubDMapping = strId + "^" + $(ctrl).closest("tr").find("input[type='text']").eq(0).val() + "|";

                PageMethods.fnSave(LoginId, UserId, NodeID, strBranchCodeMapping, strBranchNameMapping, strSubDMapping, fnSave_pass, fnFailed, strId);
            }
        }
        function fnSave_pass(res, strId) {
            if (res.split("|^|")[0] == "0") {
                fnShowReport();
                alert("Entry Updated successfully !");
            }
            else {
                fnFailed(res.split("|^|")[1]);
            }

        }
    </script>
    <style type="text/css">
        #divReport {
            overflow-y: auto;
            overflow-x: hidden;
        }

            table.clsMapping > thead > tr > th {
                color: #093D6D;
                font-size: 0.8rem;
                background: #f5f8ff;
                text-align: center;
                vertical-align: middle;
                text-transform: uppercase;
                border-bottom: 2px solid #006092 !important;
            }

            table.clsMapping tr th:nth-child(1) {
                width: 4%;
            }            
            table.clsMapping tr th:nth-child(2),
            table.clsMapping tr th:nth-child(4),
            table.clsMapping tr th:nth-child(5),
            table.clsMapping tr th:nth-child(7),
            table.clsMapping tr th:nth-child(8) {
                width: 14%;
            }
            table.clsMapping tr th:nth-child(3),
            table.clsMapping tr th:nth-child(6),
            table.clsMapping tr th:nth-child(9) {
                width: 8%;
            }

            table.clsMapping td {
                font-size: 0.8rem;
            }

            table.clsMapping td > img {
                cursor: pointer;
            }
            table.clsMapping td > select {
                padding: 2px;
            }

            table.clsMapping tr td:nth-child(1),
            table.clsMapping tr td:nth-child(3),
            table.clsMapping tr td:nth-child(9) {
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
        <%--<div style="text-align:right; padding-right: 10px;">
            <input type='button' class='btns btn-submit mt-2' style="padding: 0 20px;" value='Add New User' onclick='fnAdd()' />
        </div>--%>
        <div class="card-body pb-0" id="divHeader"></div>
        <div class="card-body" id="divReport">
        </div>
    </div>

    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>
    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
</asp:Content>

