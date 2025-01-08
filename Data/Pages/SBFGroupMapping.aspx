<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="SBFGroupMapping.aspx.cs" Inherits="SBFGroupMapping" %>

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
            $("#divRptTitle").html("SBF Group Mapping");
            $("#divContainer").height($(window).height() - ($("nav.navbar").height() + $("div.card-header").height() + 15) + "px");

            fnGetReport();
        });

        function fnTypeFilter() {
            var filter = $("#txtTypeSearchFilter").val().toUpperCase().split(",");

            if ($("#txtTypeSearchFilter").val().toUpperCase().length > 2) {
                $("#divReport").find("table").eq(0).find("tbody").eq(0).find("tr").css("display", "none");

                var flgValid = 0;
                $("#divReport").find("table").eq(0).find("tbody").eq(0).find("tr").each(function () {
                    flgValid = 1;
                    for (var t = 0; t < filter.length; t++) {
                        if ($(this).find("td").last().html().toUpperCase().indexOf(filter[t].toString().trim()) == -1) {
                            flgValid = 0;
                        }
                    }

                    if (flgValid == 1) {
                        $(this).css("display", "table-row");
                    }
                });
            }
            else {
                $("#divReport").find("table").eq(0).find("tbody").eq(0).find("tr").css("display", "table-row");
            }
        }

        function fnGetReport() {
            $("#divloader").show();

            var LoginID = $("#ConatntMatter_hdnLoginId").val();
            var UserID = $("#ConatntMatter_hdnUserId").val();
            var CategoryID = $("#ConatntMatter_ddlCategoryFilter").val();
            var BrandID = ($("#txtBrandFilter").val() == "" ? "0" : $("#txtBrandFilter").attr("selectedid"));
            var BrandFormID = ($("#txtBrandFormFilter").val() == "" ? "0" : $("#txtBrandFormFilter").attr("selectedid"));

            var flgSBFType = $("#ConatntMatter_ddlSBFType").val();
            //var flgSBFType = "1";
            //if (!($("#chkUnmapped").is(":checked")))
            //    flgSBFType = "2";

            PageMethods.GetReport(LoginID, UserID, CategoryID, BrandID, BrandFormID, flgSBFType, GetReport_pass, fnFailed);
        }
        function GetReport_pass(result) {
            $("#divloader").hide();
            $("#txtTypeSearchFilter").val("");

            if (result.split("|^|")[0] == "0") {
                $("#divReport").html(result.split("|^|")[1]);
                $("#divReport").height($(window).height() - ($("div.card-header").height() + $("#divFilterSection").height() + 160) + "px");
            }
            else {
                $("#divReport").html("Error : " + result.split("|^|")[1]);
            }
        }


        function fnCategory() {
            fnHidePopup();

            $("#txtBrandFilter").val("");
            $("#txtBrandFilter").attr("selectedid", "0");

            $("#txtBrandFormFilter").val("");
            $("#txtBrandFormFilter").attr("selectedid", "0");
        }
        function fnShowBrandPopup(ctrl) {
            fnHidePopup();

            $("#bg").show();
            $(ctrl).next().show();
            var filter = $(ctrl).val().trim().toUpperCase();
            var CategoryId = $("#ConatntMatter_ddlCategoryFilter").val();

            var str = "";
            var jsonTbl = $.parseJSON($("#ConatntMatter_hdnBrandMstr").val());
            for (var i = 0; i < jsonTbl.length; i++) {
                if (jsonTbl[i].Brand.toUpperCase().indexOf(filter) > -1) {
                    if (CategoryId == "0" || CategoryId == jsonTbl[i].CatNOdeID.toString()) {
                        str += "<div strId='" + jsonTbl[i].BrandNodeID + "' onclick='fnSelectPopupContent(this);'>";
                        str += jsonTbl[i].Brand;
                        str += "</div>";
                    }
                }
            }

            $(ctrl).next().html(str);
        }
        function fnShowBrandFormPopup(ctrl) {
            fnHidePopup();

            $("#bg").show();
            $(ctrl).next().show();
            var filter = $(ctrl).val().trim().toUpperCase();
            var CategoryId = $("#ConatntMatter_ddlCategoryFilter").val();
            var BrandID = $("#txtBrandFilter").val() == "" ? "0" : $("#txtBrandFilter").attr("selectedid");

            var str = "";
            var jsonTbl = $.parseJSON($("#ConatntMatter_hdnBrandFormMstr").val());
            for (var i = 0; i < jsonTbl.length; i++) {
                if (jsonTbl[i].BF.toUpperCase().indexOf(filter) > -1) {
                    if (BrandID != "0") {
                        if (BrandID == jsonTbl[i].BrandNodeID.toString()) {
                            str += "<div strId='" + jsonTbl[i].BFNodeID + "' onclick='fnSelectPopupContent(this);'>";
                            str += jsonTbl[i].BF;
                            str += "</div>";
                        }
                    }
                    else if (CategoryId != "0") {
                        if (CategoryId == jsonTbl[i].CatNOdeID.toString()) {
                            str += "<div strId='" + jsonTbl[i].BFNodeID + "' onclick='fnSelectPopupContent(this);'>";
                            str += jsonTbl[i].BF;
                            str += "</div>";
                        }
                    }
                    else {
                        str += "<div strId='" + jsonTbl[i].BFNodeID + "' onclick='fnSelectPopupContent(this);'>";
                        str += jsonTbl[i].BF;
                        str += "</div>";
                    }
                }
            }

            $(ctrl).next().html(str);
        }


        function fnShowGroupPopup(ctrl) {
            fnHidePopup();

            $("#bg").show();
            $(ctrl).next().show();
            var filter = $(ctrl).val().trim().toUpperCase();
            var BrandId = $(ctrl).closest("tr").attr("brandid");
            var CatNodeID = $(ctrl).closest("tr").attr("CatNodeID");

            var str = "";
            str += "<div strId='0' onclick='fnNewGroup(this);' style='background: #eaf5ff; font-weight: 800;'>New Group</div>";

            var jsonTbl = $.parseJSON($("#ConatntMatter_hdnGroupMstr").val());
            for (var i = 0; i < jsonTbl.length; i++) {
                if (jsonTbl[i].CatNodeID.toString() == "0" || jsonTbl[i].CatNodeID.toString() == CatNodeID) {
                    if (jsonTbl[i].SBFGroupName.toUpperCase().indexOf(filter) > -1) {
                        str += "<div strId='" + jsonTbl[i].SBFGroupID + "' onclick='fnSelectPopupContent(this);'>";
                        str += jsonTbl[i].SBFGroupName;
                        str += "</div>";
                    }
                }
            }

            $(ctrl).next().html(str);
        }

        function fnHidePopup() {
            $("#bg").hide();
            $("div.popup-content").hide();
        }
        function fnSelectPopupContent(ctrl) {
            $(ctrl).parent().prev().val($(ctrl).html());
            $(ctrl).parent().prev().attr("selectedid", $(ctrl).attr("strId"));

            fnHidePopup();
        }

        function fnNewGroup(ctrl) {
            fnHidePopup();

            $("#divPopup").dialog({
                title: "New Group",
                modal: true,
                width: "50%",
                open: function () {
                    var str = "<div class='p-2'>";
                    str += "<div class='mb-2'>";
                    str += "<div class='d-inline-block font-weight-bold' style='width: 20%; font-size: 1rem;'>Group Code :</div><input type='text' id='txtpopupgrpcode' class='form-control form-control-sm d-inline-block' style='width: 30%;'><span style='font-weight: 500; color: #666; margin-left: 10px;'>(Optional)</span>";
                    str += "</div>";
                    str += "<div>";
                    str += "<div class='d-inline-block font-weight-bold' style='width: 20%; font-size: 1rem;'>Group Name :</div><input type='text' id='txtpopupgrpname' class='form-control form-control-sm d-inline-block' style='width: 80%;'>";
                    str += "</div>";
                    str += "</div>";
                    $("#divPopup").html(str);
                },
                close: function () {
                    $("#divPopup").dialog('destroy');
                },
                buttons: [
                    {
                        text: "Create Group",
                        "class": "btns btn-submit",
                        click: function () {
                            var GroupID = "0";
                            var GroupCode = $("#txtpopupgrpcode").val();
                            var GroupName = $("#txtpopupgrpname").val();
                            var RoleID = $("#ConatntMatter_hdnRoleId").val();
                            var LoginID = $("#ConatntMatter_hdnLoginId").val();

                            if (GroupName == "") {
                                fnAlert("Please enter the Group Name !");
                            }
                            else {
                                $("#divloader").show();
                                PageMethods.fnAddEditGroup(GroupID, GroupCode, GroupName, RoleID, LoginID, fnAddEditGroup_pass, fnFailed, ctrl);
                                $("#divPopup").dialog('close');
                            }
                        }
                    },
                    {
                        text: "Close",
                        "class": "btns btn-submit",
                        click: function () {
                            $("#divPopup").dialog('close');
                        }
                    }
                ]
            });
        }
        function fnAddEditGroup_pass(result, ctrl) {
            $("#divloader").hide();
            if (result.split("|^|")[0] == "0") {
                $(ctrl).parent().prev().val(result.split("|^|")[2]);
                $(ctrl).parent().prev().attr("selectedid", result.split("|^|")[1]);
                $("#ConatntMatter_hdnGroupMstr").val(result.split("|^|")[3]);

                //fnUpdateMapping($(ctrl).parent().closest("td").next().find("a")[0]);
            }
            else {
                //$("#divloader").hide();
                fnAlert("Error : " + result.split("|^|")[1]);
            }
        }

        function fnCheckAll(ctrl) {
            if ($(ctrl).is(":checked"))
                $(ctrl).closest("table").find("tbody").eq(0).find("input[type='checkbox']").prop("checked", true);
            else
                $(ctrl).closest("table").find("tbody").eq(0).find("input[type='checkbox']").prop("checked", false);
        }
        function fnCheckIndividual(ctrl) {
            if (!($(ctrl).is(":checked")))
                $(ctrl).closest("table").find("thead").eq(0).find("input[type='checkbox']").prop("checked", false);
        }


        function fnUpdateMapping(ctrl) {
            var SBF = $(ctrl).closest("tr").find("td").eq(3).html();
            if ($(ctrl).closest("tr").find("input[type='text']").eq(0).val() == "" || $(ctrl).closest("tr").find("input[type='text']").eq(0).attr("selectedid") == "0") {
                fnAlert("Please select or create a new Group for mapping the SBF - " + SBF + " !");
            }
            else {
                var UserID = $("#ConatntMatter_hdnUserId").val();
                var LoginID = $("#ConatntMatter_hdnLoginId").val();
                var strSBFGroiPMapping = $(ctrl).closest("tr").attr("sbfid") + "^" + $(ctrl).closest("tr").find("input[type='text']").eq(0).attr("selectedid") + "|";

                $("#divloader").show();
                PageMethods.fnUpdateMapping(strSBFGroiPMapping, UserID, LoginID, fnUpdateMapping_pass, fnFailed, SBF);
            }
        }
        function fnUpdateMapping_pass(result, SBF) {
            if (result.split("|^|")[0] == "0") {
                fnGetReport();
                fnAlert(SBF + " mapped successfully !");
            }
            else {
                $("#divloader").hide();
                fnAlert("Error : " + result.split("|^|")[1]);
            }
        }

        function fnUpdateMultipleMapping() {
            if ($("#divReport").find("table").eq(0).find("tbody").eq(0).find("tr").length > 0) {

                var cntr = 0;
                var strSBFGroiPMapping = "";
                $("#divReport").find("table").eq(0).find("tbody").eq(0).find("tr").each(function () {
                    if ($(this).find("input[type='text']").eq(0).val() != "" && $(this).find("input[type='text']").eq(0).attr("selectedid") != "0") {
                        strSBFGroiPMapping += $(this).attr("sbfid") + "^" + $(this).find("input[type='text']").eq(0).attr("selectedid") + "|";
                        cntr++;
                    }
                });

                if (cntr == 0) {
                    fnAlert("Please select atleast one SBF-Group mapping for Action !");
                }
                else {
                    $("#divAlert").dialog({
                        title: "Confirmation Alert :",
                        modal: true,
                        width: "50%",
                        open: function () {
                            $("#divAlert").html("Are you sure, you are going to update " + cntr + " SBF-Group(s) mapping ?");
                        },
                        close: function () {
                            $("#divAlert").dialog('destroy');
                        },
                        buttons: [
                            {
                                text: "Update",
                                "class": "btns btn-submit",
                                click: function () {
                                    var UserID = $("#ConatntMatter_hdnUserId").val();
                                    var LoginID = $("#ConatntMatter_hdnLoginId").val();

                                    $("#divloader").show();
                                    PageMethods.fnUpdateMapping(strSBFGroiPMapping, UserID, LoginID, fnUpdateMultipleMapping_pass, fnFailed, cntr);

                                    $("#divAlert").dialog('close');
                                }
                            },
                            {
                                text: "Cancel",
                                "class": "btns btn-submit",
                                click: function () {
                                    $("#divAlert").dialog('close');
                                }
                            }
                        ]
                    });
                }
            }
            else
                fnAlert("No SBF(s) found for Action !");
        }
        function fnUpdateMultipleMapping_pass(result, Cntr) {
            if (result.split("|^|")[0] == "0") {
                fnShowReport();
                fnAlert(Cntr + " SBF-Group(s) mapping updated successfully !");
            }
            else {
                $("#divloader").hide();
                fnAlert("Error : " + result.split("|^|")[1]);
            }
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
        function fnDownloadTemplate() {
            $("#ConatntMatter_btnDownloadTemplate").click();
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

        .clslbl {
            font-size: 0.9rem;
            font-weight: 600;
            color: #666;
        }

        #bg {
            z-index: 2;
            background: transparent;
        }

        div.popup-content {
            z-index: 3;
            display: none;
            padding: 3px 0;
            min-width: 210px;
            max-height: 200px;
            overflow-y: auto;
            position: absolute;
            font-size: 0.76rem;
            background: #fbfdff;
            border: 1px solid #ddd;
        }

            div.popup-content > div {
                cursor: pointer;
                text-align: left;
                padding: 1px 8px;
                border-bottom: 1px solid #ddd;
            }

                div.popup-content > div:hover {
                    font-size: 0.8rem;
                    font-weight: 700;
                    background: #ddf0ff;
                }


        .tbl-control-txt {
            font-size: 0.875rem !important;
            border-radius: .2rem !important;
            height: calc(1em + .5rem + 2px) !important;
        }
    </style>
    <style type="text/css">
        table > thead > tr > th {
            color: #093D6D;
            background: #f5f8ff;
            text-align: center;
            vertical-align: middle;
            text-transform: uppercase;
            border-bottom: 2px solid #006092 !important;
        }

        table.cls-SBFGrpMapping tr td:nth-child(1) {
            width: 40px;
            text-align: center;
        }

        table.cls-SBFGrpMapping tr td:nth-child(2) {
            width: 13%;
            text-align: left;
            padding-left: 5px;
        }

        table.cls-SBFGrpMapping tr td:nth-child(3),
        table.cls-SBFGrpMapping tr td:nth-child(4) {
            width: 18%;
            text-align: left;
            padding-left: 5px;
            white-space: nowrap;
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

        <div id="divContainer" class="row">
            <div class="col-12">
                <div id="divFilterSection" class="m-2 pt-2 pl-4 pb-2 pr-4" style="border: 1px solid #00b300; border-radius: 4px; background: #f9fff9;">
                    <div class="row">
                        <div class="col-8">
                            <div class="row">
                                <div class="col-2 pl-0 pr-0">
                                    <span class="clslbl">SBF Type : </span>
                                    <asp:DropDownList ID="ddlSBFType" runat="server" CssClass="form-control form-control-sm" onchange="fnCategory();">
                                        <asp:ListItem Text="New Mapping" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Existing Mapping" Value="2"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-2 pr-0">
                                    <span class="clslbl">Category : </span>
                                    <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="form-control form-control-sm" onchange="fnCategory();"></asp:DropDownList>
                                </div>
                                <div class="col-3 pr-0">
                                    <span class="clslbl">Brand : </span>
                                    <div style="position: relative;">
                                        <input id="txtBrandFilter" type="text" class="form-control form-control-sm" selectedid="0" onkeyup="fnShowBrandPopup(this);" onclick="fnShowBrandPopup(this);" />
                                        <div class='popup-content popup-content-brand'></div>
                                    </div>
                                </div>
                                <div class="col-3 pr-0">
                                    <span class="clslbl">BrandForm : </span>
                                    <div style="position: relative;">
                                        <input id="txtBrandFormFilter" type="text" class="form-control form-control-sm" selectedid="0" onkeyup="fnShowBrandFormPopup(this);" onclick="fnShowBrandFormPopup(this);" />
                                        <div class='popup-content popup-content-brandform'></div>
                                    </div>
                                </div>
                                <%--<div class="col-2 pt-4 pr-0">
                                    <input type="checkbox" id="chkUnmapped" checked />
                                    <span class="clslbl">SBF Not Mapped</span>
                                </div>--%>
                                <div class="col-2 pt-4 pr-0 ">
                                    <a id="btnGetReport" class="btn btn-primary btn-sm" href="#" onclick="fnGetReport();" title="Get Filtered Report">Get Report</a>
                                </div>
                            </div>
                        </div>
                        <div class="col-4">
                            <span class="clslbl">Search : </span>
                            <input id="txtTypeSearchFilter" type="text" class="form-control form-control-sm" onkeyup="fnTypeFilter();" placeholder="Type atleast 3 characters.." />
                        </div>

                        <div class="col-12" style="display: none;">
                            <asp:Button ID="btnDownloadTemplate" runat="server" Text="." Style="visibility: hidden;" title="Click to download" OnClick="btnDownloadTemplate_Click" />
                            <a href="#" onclick="fnDownloadTemplate();">
                                <img class="mr-2" src="../../Images/excel.gif" style="height: 18px;" />Download</a>
                        </div>
                    </div>
                </div>
                <div class="m-2 pt-0 text-center">
                    <div id="divReport" class="mt-2" style="width: 99%; font-size: 0.9rem; overflow: auto;"></div>
                </div>
            </div>
        </div>
    </div>

    <div class="row footer-btn pt-1 pb-1">
        <div id="divbtns" class="col-12 text-right">
            <a href="#" class="btn btn-info btn-sm" style="font-weight: 600; padding: 0.25rem 1rem;" onclick="fnUpdateMultipleMapping();">Save All</a>
        </div>
    </div>

    <div id="divPopup" style="display: none;"></div>
    <div id="divAlert" style="display: none;"></div>
    <div id="divSubAlert" style="display: none;"></div>

    <div id="bg" class="loader_bg" onclick="fnHidePopup();"></div>
    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>

    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnRoleId" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />
    <asp:HiddenField ID="hdnUserName" runat="server" />

    <asp:HiddenField ID="hdnGroupMstr" runat="server" />
    <asp:HiddenField ID="hdnBrandMstr" runat="server" />
    <asp:HiddenField ID="hdnBrandFormMstr" runat="server" />
</asp:Content>

