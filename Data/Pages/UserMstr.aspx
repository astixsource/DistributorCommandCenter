<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="UserMstr.aspx.cs" Inherits="UserMstr" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />--%>

    <script type="text/javascript">
        function fnRefreshPage() {
            fnShowReport();
        }

        function fnFailed(result) {
            alert("Error : " + result);

            $("#divloader").hide();
        }

        $(document).ready(function () {
            $("#divRptTitle").html("User Master");

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

                var wid = $("#tblUser").width();
                var thead = $("#tblUser").find("thead").eq(0).html();
                $("#divHeader").html("<table id='tblUser_header' class='table table-bordered table-sm clsUser' style='margin-top:-4px; margin-bottom:0; width:" + (wid - 1) + "px; min-width:" + (wid - 1) + "px;'><thead>" + thead + "</thead></table>");
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

                $("#divReport").height($(window).height() - ($("div.card-header").height() + $("#tblUser_header")[0].offsetHeight + 160) + "px");
            }
            else {
                $("#divHeader").html("");
                $("#divReport").html("Error : " + result.split("|^|")[1]);
            }
        }

        function fnAdd() {
            var str = "<tr flgEdit='1' UserID='0'>";
            str += "<td></td>";
            str += "<td><input type='text' class='form-control form-control-sm'/></td>";
            str += "<td><input type='text' class='form-control form-control-sm'/></td>";
            str += "<td><input type='checkbox' checked/></td>";
            str += "<td><a href='#' class='btn btn-primary btn-xs' selectedstr='' onclick='fnBranchPopup(this);'/>Click to select</a></td>";
            str += "<td data-title='actionbtn'><img title='Save' src='../../Images/save_blue.png' onclick='fnIndividualSave(this);'/><img title='Cancel' src='../../Images/cancel_blue.png' class='ml-2' onclick='fnCancel(this);'/></td>";
            str += "</tr>";

            $("#tblUser").find("tbody").eq(0).prepend(str);
        }
        function fnEdit(ctrl) {
            var strName = $(ctrl).closest("tr").attr("UserName");
            var strEmail = $(ctrl).closest("tr").attr("UserEmailID");
            var strActive = $(ctrl).closest("tr").attr("Active");
            var strBranches = $(ctrl).closest("tr").attr("Branches");

            $(ctrl).closest("tr").attr("flgEdit", "1");

            $(ctrl).closest("tr").find("td").eq(1).html("<input type='text' class='form-control form-control-sm' value='" + strName + "'/>");
            $(ctrl).closest("tr").find("td").eq(2).html("<input type='text' class='form-control form-control-sm' value='" + strEmail + "'/>");

            if (strActive.trim().toUpperCase() == "YES")
                $(ctrl).closest("tr").find("td").eq(3).html("<input type='checkbox' checked/>");
            else
                $(ctrl).closest("tr").find("td").eq(3).html("<input type='checkbox'/>");

            $(ctrl).closest("tr").find("td").eq(4).html("<a href='#' class='btn btn-primary btn-xs' selectedstr='" + strBranches + "' onclick='fnBranchPopup(this);'/>Click to select</a>");
            $(ctrl).closest("td").eq(0).html("<img title='Save' src='../../Images/save_blue.png' onclick='fnIndividualSave(this);'/><img title='Cancel' src='../../Images/cancel_blue.png' class='ml-2' onclick='fnCancel(this);'/>");
        }
        function fnCancel(ctrl) {
            if ($(ctrl).closest("tr").attr("UserID") == "0") {
                $(ctrl).closest("tr").remove();
            }
            else {
                var strName = $(ctrl).closest("tr").attr("UserName");
                var strEmail = $(ctrl).closest("tr").attr("UserEmailID");
                var strActive = $(ctrl).closest("tr").attr("Active");
                var BranchCount = $(ctrl).closest("tr").attr("BranchCount");

                $(ctrl).closest("tr").attr("flgEdit", "0");
                $(ctrl).closest("tr").find("td").eq(1).html(strName);
                $(ctrl).closest("tr").find("td").eq(2).html(strEmail);
                $(ctrl).closest("tr").find("td").eq(3).html(strActive);
                $(ctrl).closest("tr").find("td").eq(4).html(BranchCount);
                $(ctrl).closest("td").html("<img src='../../Images/edit_blue.png' alt='edit' onclick='fnEdit(this)'/>");
            }
        }
        function fnIndividualSave(ctrl) {
            var strName = $(ctrl).closest("tr").find("td").eq(1).find("input").eq(0).val();
            var strEmail = $(ctrl).closest("tr").find("td").eq(2).find("input").eq(0).val();
            var branch = $(ctrl).closest("tr").find("td").eq(4).find("a.btn").eq(0).attr("selectedstr");

            if (strName == "") {
                alert("Please enter the User Name !");
            }
            else if (strEmail == "") {
                alert("Please enter the User Email-Id !");
            }
            else if (!strEmail.match(/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/)) {
                alert("User Email-Id is not valid !");
            }
            else if (branch == "") {
                alert("Please select the Branches !");
            }
            else {
                $("#divloader").show();

                var strId = $(ctrl).closest("tr").attr("UserID");
                var flgActive = $(ctrl).closest("tr").find("td").eq(3).find("input[type='checkbox']").eq(0).is(":checked") == true ? 1 : 0;

                PageMethods.fnSave(strId, strName, strEmail, flgActive, branch, fnSave_pass, fnFailed, strId);
            }
        }
        function fnSave_pass(res, strId) {
            if (res.split("|^|")[0] == "0") {
                alert("Entry Updated successfully !");
                fnShowReport();
            }
            else {
                fnFailed(res.split("|^|")[1]);
            }
        }
    </script>
    <script type="text/javascript">
        function fnBranchPopup(ctrl) {
            $("#divBranchHier").dialog({
                title: "Branches :",
                modal: true,
                width: "50%",
                height: 400,
                open: function () {
                    $("#divBranchHier").html($("#ConatntMatter_hdnBranchHierMstr").val());

                    if ($(ctrl).attr("selectedstr") != "") {
                        var selectionArr = $(ctrl).attr("selectedstr").split(",");
                        for (var i = 0; i < selectionArr.length - 1; i++) {
                            var chk = $("#divBranchHier").find("input[type='checkbox'][code='" + selectionArr[i].split("^")[0] + "'][ntype='" + selectionArr[i].split("^")[1] + "']");
                            chk.prop("checked", true);

                            if (chk.attr("IslastNode") == "0")
                                chk.closest("li").find("ul").eq(0).find("input[type='checkbox']").prop("disabled", true);
                        }
                    }
                },
                close: function () {
                    $("#divBranchHier").dialog('destroy');
                },
                buttons: [
                    {
                        text: "Submit",
                        "class": "btns btn-submit",
                        click: function () {
                            var selectedstr = "";
                            $("#divBranchHier").find("input[type='checkbox']:checked").each(function () {
                                selectedstr += $(this).attr("code") + "^" + $(this).attr("ntype") + ",";
                            });

                            $(ctrl).attr("selectedstr", selectedstr);
                            $("#divBranchHier").dialog('close');
                        }
                    },
                    {
                        text: "Close",
                        "class": "btns btn-submit",
                        click: function () {
                            $("#divBranchHier").dialog('close');
                        }
                    }
                ]
            });
        }

        function fnExpand(ctrl) {
            $(ctrl).closest("li").find("ul").eq(0).slideDown();
            $(ctrl).attr("title", "Collapse");
            $(ctrl).attr("src", "../../Images/icoMinus.gif");
            $(ctrl).attr("onclick", "fnCollapse(this);");
        }
        function fnCollapse(ctrl) {
            $(ctrl).closest("li").find("ul").eq(0).slideUp();
            $(ctrl).attr("title", "Expand");
            $(ctrl).attr("src", "../../Images/icoAdd.gif");
            $(ctrl).attr("onclick", "fnExpand(this);");
        }

        function fnCheckNode(ctrl) {
            var IsValidlvl = true;
            var ntype = $(ctrl).attr("ntype");
            if ($("#divBranchHier").find("input[type='checkbox']:checked").length > 1) {
                $("#divBranchHier").find("input[type='checkbox']:checked").each(function fn() {
                    if ($(this).attr("ntype") != ntype)
                        IsValidlvl = false;
                });
            }

            if (IsValidlvl) {
                var IslastNode = $(ctrl).attr("IslastNode");
                if (IslastNode == "0") {
                    if ($(ctrl).is(":checked")) {
                        $(ctrl).closest("li").find("ul").eq(0).find("input[type='checkbox']").prop("disabled", true);
                    }
                    else {
                        $(ctrl).closest("li").find("ul").eq(0).find("input[type='checkbox']").prop("disabled", false);
                    }
                }
            }
            else {
                alert("Please make your selection at same level !");
                $(ctrl).prop("checked", false);
            }
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
    </style>
    <style type="text/css">
        #divBranchHier ul {
            list-style: none;
        }

        #divBranchHier li > img {
            width: 10px;
            cursor: pointer;
            margin-right: 5px;
            margin-bottom: 6px;
        }
    </style>
    <style type="text/css">
        #divReport {
            overflow-y: auto;
            overflow-x: hidden;
        }

        table.clsUser {
            width: 75%;
            margin: 0 0 0 12%;
        }

            table.clsUser > thead > tr > th {
                color: #093D6D;
                font-size: 0.8rem;
                background: #f5f8ff;
                text-align: center;
                vertical-align: middle;
                text-transform: uppercase;
                border-bottom: 2px solid #006092 !important;
            }

            table.clsUser tr th:nth-child(1) {
                width: 8%;
            }

            table.clsUser tr th:nth-child(2),
            table.clsUser tr th:nth-child(3) {
                width: 28%;
            }

            table.clsUser tr th:nth-child(4),
            table.clsUser tr th:nth-child(5),
            table.clsUser tr th:nth-child(6) {
                width: 12%;
            }

            table.clsUser td {
                font-size: 0.8rem;
                vertical-align: middle;
            }

                table.clsUser td > img {
                    cursor: pointer;
                }

            table.clsUser tr td:nth-child(1),
            table.clsUser tr td:nth-child(4),
            table.clsUser tr td:nth-child(5),
            table.clsUser tr td:nth-child(6) {
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
        <div style="text-align: right; padding-right: 10px;">
            <input type='button' class='btns btn-submit mt-2' style="padding: 0 20px;" value='Add New User' onclick='fnAdd()' />
        </div>
        <div class="card-body pb-0" id="divHeader"></div>
        <div class="card-body pt-0" id="divReport">
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

