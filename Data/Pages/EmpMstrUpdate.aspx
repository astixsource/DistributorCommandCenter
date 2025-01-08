<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="EmpMstrUpdate.aspx.cs" Inherits="EmpMstrUpdate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />--%>

    <script type="text/javascript">
        var MonthArr = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        function fnGetCurrentDat() {
            var d = new Date();
            return d.getDate() + "-" + MonthArr[d.getMonth()] + "-" + d.getFullYear();
        }

        function fnDateDiff(start_date, end_date) {
            if (start_date != "" && end_date != "") {
                var startDate = $.datepicker.parseDate("dd-M-yy", start_date);
                var endDate = $.datepicker.parseDate("dd-M-yy", end_date);
                var timeDiff = endDate - startDate;
                return Math.floor(timeDiff / (1000 * 60 * 60 * 24));
            }
        }

        function fnFailed(result) {
            $("#divloader").hide();
            fnAlert("Error : " + result);
        }
        $(document).ready(function () {
            $("#divRptTitle").html("Employee Master Update");
            $("#divContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 15) + "px");

            //fnShowReport();
        });

        function fnShowReport() {
            $("#divloader").show();
            var UserID = $("#ConatntMatter_hdnUserId").val();
            var LoginID = $("#ConatntMatter_hdnLoginId").val();

            PageMethods.GetReport(UserID, LoginID, GetReport_pass, fnFailed);
        }
        function GetReport_pass(result) {
            $("#divloader").hide();

            if (result.split("|^|")[0] == "0") {
                $("#ConatntMatter_divReport").html(result.split("|^|")[1]);
            }
            else {
                $("#ConatntMatter_divReport").html("Error : " + result.split("|^|")[1]);
            }
        }

        function fnViewBranchData(ctrl) {
            $("#divloader").show();
            var UserID = $("#ConatntMatter_hdnUserId").val();
            var LoginID = $("#ConatntMatter_hdnLoginId").val();
            var Month = $("#ConatntMatter_ddlMonthYear").val().split("|")[0];
            var Year = $("#ConatntMatter_ddlMonthYear").val().split("|")[1];

            var ChannelChangeBranchDataID = $(ctrl).closest("tr").attr("strid");
            $("#ConatntMatter_hdnChannelChangeBranchDataID").val(ChannelChangeBranchDataID);

            var Branchname = $(ctrl).closest("tr").find("td").eq(1).html();
            PageMethods.BranchWiseReport(UserID, Month, Year, LoginID, ChannelChangeBranchDataID, BranchWiseReport_pass, fnFailed, Branchname);
        }
        function BranchWiseReport_pass(result, Branchname) {
            $("#divloader").hide();

            if (result.split("|^|")[0] == "0") {
                $("#divBranchWiseReport").dialog({
                    title: Branchname + " Data :",
                    modal: true,
                    width: "90%",
                    height: "560",
                    open: function () {
                        $("#divBranchWiseReportTbl").html(result.split("|^|")[1]);
                    },
                    close: function () {
                        $("#divBranchWiseReport").dialog('destroy');
                    },
                    buttons: [
                        {
                            text: "Close",
                            "class": "btns btn-submit",
                            click: function () {
                                $("#divBranchWiseReport").dialog('close');
                            }
                        }
                    ]
                });
            }
            else {
                $("#ConatntMatter_divReport").html("Error : " + result.split("|^|")[1]);
            }
        }



        function fnSelectFile() {
            var obj = $("#FileUploader").val().split("\\");
            var filename = obj[obj.length - 1];
            $("#FileUploader").next().html(filename);
        }
        function fnUploadData() {
            fnAlert("Coming soon !");
            //if ($("#FileUploader").get(0).files.length == 0) {
            //    fnAlert("Please Select the File !");
            //}
            //else {
            //    var file = $("#FileUploader").get(0).files;

            //    var data = new FormData();
            //    data.append(file[0].name, file[0]);
            //    data.append("FileType", "2");
            //    data.append("UserID", $("#ConatntMatter_hdnUserId").val());
            //    data.append("LoginID", $("#ConatntMatter_hdnLoginId").val());
            //    data.append("MonthYr", "0|0");

            //    //$("#divloader").show();
            //    $("#divSubAlert").dialog({
            //        title: "Alert Message :",
            //        modal: true,
            //        width: "50%",
            //        open: function () {
            //            $("#divSubAlert").html("Please wait while Employee Master is updating ...");
            //        },
            //        close: function () {
            //            $("#divSubAlert").dialog('destroy');
            //        }
            //    });

            //    $.ajax({
            //        url: "../../MstrUpdater.ashx",
            //        type: "POST",
            //        data: data,
            //        async: true,
            //        contentType: false,
            //        processData: false,
            //        success: function (result) {
            //            $("#divSubAlert").dialog('close');
            //            fnAlert(result.split("^")[1]);
            //            $("#divloader").hide();

            //            if (result.split("^")[0] == "0") {
            //                $("#ConatntMatter_hdnFileID").html(result.split("^")[2]);
            //                $("#FileUploader").next().html('');
            //                $("#FileUploader").val('');
            //            }
            //            else if (result.split("^")[0] == "2") {
            //                $("#FileUploader").next().html('');
            //                $("#FileUploader").val('');

            //                $("#ConatntMatter_btnDownloadErrorRpt").click();
            //            }

            //        },
            //        error: function (err) {
            //            $("#divSubAlert").dialog('close');
            //            fnAlert("Error : " + err.statusText);
            //            $("#divloader").hide();
            //        }
            //    });
            //}
        }


        function fnAlert(msg) {
            $("#divAlert").dialog({
                title: "Alert Message :",
                modal: true,
                width: "50%",
                open: function () {
                    $("#divAlert").html(msg);
                },
                close: function () {
                    $("#divAlert").dialog('destroy');
                },
                buttons: [
                    {
                        text: "Close",
                        "class": "btns btn-submit",
                        click: function () {
                            $("#divAlert").dialog('close');
                        }
                    }
                ]
            });
        }
    </script>

    <style type="text/css">
        @media (min-width: 1250px) {
            .container {
                max-width: 1250px;
            }
        }

        .main-content {
            padding-bottom: 0 !important;
        }

        .clslbl {
            font-size: 0.9rem;
            font-weight: 600;
        }

        .footer-btn {
            padding: 5px 10px;
            background-color: #ddd;
            border-top: 1px solid #bbb;
            position: fixed;
            bottom: 0;
            left: 0;
            margin: 0;
            width: 100%;
        }


        table > thead > tr > th {
            color: #093D6D;
            background: #f5f8ff;
            text-align: center;
            vertical-align: middle;
            text-transform: uppercase;
            font-size: 0.76rem !important;
            border-bottom: 2px solid #006092 !important;
        }

        table > tbody > tr > td {
            font-size: 0.76rem !important;
        }
    </style>
    <style type="text/css">
        table.cls-BranchData tr td:nth-child(1) {
            width: 30px;
            text-align: center;
        }

        table.cls-BranchData tr td:nth-child(2),
        table.cls-BranchData tr td:nth-child(5) {
            width: 20%;
            text-align: left;
            padding-left: 5px;
        }

        table.cls-BranchData tr td:nth-child(3),
        table.cls-BranchData tr td:nth-child(6),
        table.cls-BranchData tr td:nth-child(7) {
            white-space: nowrap;
        }

        table.cls-OtherUserBranch tr td {
            text-align: center;
            font-size: 0.76rem !important;
        }

            table.cls-OtherUserBranch tr td:nth-child(2),
            table.cls-OtherUserBranch tr td:nth-child(3) {
                width: 30%;
                text-align: left;
                padding-left: 5px;
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
            <div class="w-75 m-auto">
                <div id="divUploadSection" class="ml-2 mt-2 mb-2 p-4" style="border: 1px solid #00b300; border-radius: 4px; background: #f9fff9;">
                    <div id="divUploadSubSection" class="row">
                        <div class="col-10">
                            <span class="clslbl">Select File : </span>
                            <input type="file" id="FileUploader" name="FileUploader" onchange="fnSelectFile();" class="form-control-sm" style="width: 105px; padding-left: 20px;" />
                            <span></span>
                        </div>
                        <div class="col-2 pr-4 text-right">
                            <input id="btnUpload" type='button' class='btns btn-submit pt-1 pr-3 pb-1 pl-3' style="font-size: 0.8rem;" title="Click to Upload" value='Upload' onclick='fnUploadData();' />                            
                            <asp:Button ID="btnDownloadErrorRpt" runat="server" Text="." Style="visibility: hidden;" title="Click to download Error Report" OnClick="btnDownloadErrorRpt_Click" />
                        </div>
                    </div>
                </div>
                <div class="w-100 m-2 pt-0 text-center">
                    <div id="divReport" runat="server" class="w-100 mt-2 ml-auto mr-auto" style="font-size: 0.9rem; overflow: auto;"></div>
                    <%--<div id="divMsg" runat="server" class="w-50 mt-4 ml-auto mr-auto" style="color: #ff0000; font-size: 0.9rem; font-weight: 600;"></div>--%>
                </div>
            </div>
        </div>
    </div>
    <div class="row footer-btn pt-1 pb-1">
        <div id="divbtns" class="col-12 text-right">
            <a href="#" class="btn btn-info btn-sm" style="font-weight: 600; padding: 0.25rem 1rem;" onclick="fnCommitMultiple();">Commit Selected</a>
        </div>
    </div>

    <div id="divAlert" style="display: none;"></div>
    <div id="divSubAlert" style="display: none;"></div>
    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>

    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />

    <asp:HiddenField ID="hdnFileID" runat="server" />
    <asp:HiddenField ID="hdnChannelChangeBranchDataID" runat="server" />
</asp:Content>

