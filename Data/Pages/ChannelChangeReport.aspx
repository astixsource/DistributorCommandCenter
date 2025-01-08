<%@ Page Title="" Language="C#" MasterPageFile="~/Data/MasterPage/Site.master" AutoEventWireup="true" CodeFile="ChannelChangeReport.aspx.cs" Inherits="ChannelChangeReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<script src="../../BICss/lib/jquery/dist/jquery.min.js"></script>
    <link href="../../BICss/style/style.css" rel="stylesheet" />--%>

    <script type="text/javascript">
        function fnRefreshPage() {
            fnShowReport();
        }

        function fnFailed(result) {
            $("#divloader").hide();
            alert("Error : " + result);
        }

        $(document).ready(function () {
            $("#divRptTitle").html("Channel Change Report");

            fnShowReport();
        });

        function fnShowReport() {
            $("#divloader").show();
            var Month = $("#ConatntMatter_ddlMonthYear").val().split("|")[0];
            var Year = $("#ConatntMatter_ddlMonthYear").val().split("|")[1];
            var UserID = $("#ConatntMatter_hdnUserId").val();
            var LoginID = $("#ConatntMatter_hdnLoginId").val();

            PageMethods.GetReport(Month, Year, UserID, LoginID, GetReport_pass, fnFailed);
        }
        function GetReport_pass(result) {
            $("#divloader").hide();

            if (result.split("|^|")[0] == "0") {
                $("#divReport").html(result.split("|^|")[1]);

                var wid = $("#tblChannel").width();
                var thead = $("#tblChannel").find("thead").eq(0).html();
                $("#divHeader").html("<table id='tblChannel_header' class='table table-bordered table-sm clsChannel' style='margin-top:-4px; margin-bottom:0; width:" + (wid - 1) + "px; min-width:" + (wid - 1) + "px;'><thead>" + thead + "</thead></table>");
                $("#tblChannel").css("width", wid);

                $("#tblChannel").css("min-width", wid);
                for (i = 0; i < $("#tblChannel").find("th").length - 1; i++) {
                    var th_wid = $("#tblChannel").find("th")[i].clientWidth;
                    $("#tblChannel_header").find("th").eq(i).css("min-width", th_wid);
                    $("#tblChannel_header").find("th").eq(i).css("width", th_wid);
                    $("#tblChannel").find("th").eq(i).css("min-width", th_wid);
                    $("#tblChannel").find("th").eq(i).css("width", th_wid);
                }
                $("#tblChannel").css("margin-top", "-" + $("#tblChannel_header")[0].offsetHeight + "px");

                $("#divReport").height($(window).height() - ($("div.card-header").height() + $("#tblChannel_header")[0].offsetHeight + 170) + "px");
            }
            else {
                $("#divHeader").html("");
                $("#divReport").html("Error : " + result.split("|^|")[1]);
            }
        }
    </script>

    <%--Not In Use--%>
    <script type="text/javascript">
        function fnEdit(ctrl) {
            $(ctrl).closest("tr").attr("flgEdit", "1");
            $(ctrl).closest("tr").attr("NewImg", $(ctrl).closest("tr").attr("Img"));

            $(ctrl).closest("tr").find("select").removeAttr("disabled");
            $(ctrl).closest("td").eq(0).html("<img title='Save' src='../../Images/save_blue.png' onclick='fnIndividualSave(this);'/><img title='Cancel' src='../../Images/cancel_blue.png' class='ml-2' onclick='fnCancel(this);'/>");
        }
        function fnCancel(ctrl) {
            $(ctrl).closest("tr").attr("flgEdit", "0");
            $(ctrl).closest("tr").attr("IsInTemp", "0");
            $(ctrl).closest("tr").attr("NewImg", $(ctrl).closest("tr").attr("Img"));

            $(ctrl).closest("tr").find("select").val($(ctrl).closest("tr").attr("IsJBPDone"));
            $(ctrl).closest("tr").find("select").prop("disabled", true);

            $(ctrl).closest("td").eq(0).html("<img src='../../Images/edit_blue.png' alt='edit' onclick='fnEdit(this)'/>");
        }
        function fnIndividualSave(ctrl) {
            var strId = $(ctrl).closest("tr").attr("strId");

            if ($(ctrl).closest("tr").attr("NewImg") == "") {
                alert("Please select the Image !");
            }
            else {
                $("#divloader").show();
                var Month = $("#ConatntMatter_ddlMonthYear").val().split("-")[0];
                var Year = $("#ConatntMatter_ddlMonthYear").val().split("-")[1];
                var NodeID = $("#ConatntMatter_hdnNodeID").val();
                var strJBPData = strId + "^" + $(ctrl).closest("tr").attr("NewImg") + "^" + $(ctrl).closest("tr").find("select").eq(0).val() + "^|";
                var LoginId = $("#ConatntMatter_hdnLoginId").val();

                PageMethods.fnSave(Month, Year, NodeID, strJBPData, LoginId, fnSave_pass, fnFailed, $(ctrl).closest("tr").attr("NewImg"));
            }
        }
        function fnSave_pass(res, Img) {
            if (res.split("|^|")[0] == "0") {
                fnMovefromTemp(Img);
                fnShowReport();
                alert("Entry Updated successfully !");
            }
            else {
                fnFailed(res.split("|^|")[1]);
            }

        }

        function fnImg(ctrl) {
            $("#divImage").dialog({
                title: $(ctrl).closest("tr").find("td").eq(4).html() + " :",
                modal: true,
                width: "50%",
                open: function () {
                    $("#ConatntMatter_hdnBranchSubDNodeID").val($(ctrl).closest("tr").attr("strId"));

                    if ($(ctrl).closest("tr").attr("flgEdit") == "0")
                        $("#btnUpload").hide();
                    else
                        $("#btnUpload").show();

                    if ($(ctrl).closest("tr").attr("flgEdit") == "0")
                        $("#JBPImg").attr("src", "../../Files/JBP/" + $(ctrl).closest("tr").attr("Img"));
                    else if ($(ctrl).closest("tr").attr("NewImg") == "")
                        $("#JBPImg").attr("src", "../../Files/no-image.png");
                    else if ($(ctrl).closest("tr").attr("IsInTemp") == "0")
                        $("#JBPImg").attr("src", "../../Files/JBP/" + $(ctrl).closest("tr").attr("NewImg"));
                    else
                        $("#JBPImg").attr("src", "../../Files/JBP/Temp/" + $(ctrl).closest("tr").attr("NewImg"));
                },
                close: function () {
                    $("#divImage").dialog('destroy');
                },
                buttons: [
                    {
                        text: "Close",
                        "class": "btns btn-submit",
                        click: function () {
                            $("#divImage").dialog('close');
                        }
                    }
                ]
            });
        }
        function fnClicktoUploadImg() {
            $("#fileUploader").click();
        }
        function fnUploadImg() {
            if ($("#fileUploader").get(0).files.length > 0) {
                var LoginId = $("#ConatntMatter_hdnLoginId").val();
                var UserId = $("#ConatntMatter_hdnUserId").val();
                var NodeID = $("#ConatntMatter_hdnNodeID").val();

                var data = new FormData();
                var file = $("#fileUploader").get(0).files;
                data.append(file[0].name, file[0]);
                data.append("UserID", UserId);
                data.append("NodeID", NodeID);
                data.append("LoginID", LoginId);
                data.append("FileID", $("#ConatntMatter_hdnBranchSubDNodeID").val());
                data.append("FileName", "");
                data.append("Actionflg", "1");

                $("#divloader").show();
                $.ajax({
                    url: "../../FileUploaderHandler.ashx",
                    type: "POST",
                    data: data,
                    async: true,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.split("^")[0] == "0") {
                            $("#fileUploader").val('');
                            $("#JBPImg").attr("src", "../../Files/JBP/Temp/" + result.split("^")[1]);
                            $("#tblJBP").find("tbody").eq(0).find("tr[strId='" + $("#ConatntMatter_hdnBranchSubDNodeID").val() + "']").eq(0).attr("IsInTemp", "1");
                            $("#tblJBP").find("tbody").eq(0).find("tr[strId='" + $("#ConatntMatter_hdnBranchSubDNodeID").val() + "']").eq(0).attr("NewImg", result.split("^")[1]);
                        }
                        else
                            alert("Error :" + result.split("^")[1]);

                        $("#divloader").hide();
                    },
                    error: function (err) {
                        alert("Error :" + err.statusText);
                        $("#divloader").hide();
                    }
                });
            }
        }
        function fnMovefromTemp(ImgName) {
            var LoginId = $("#ConatntMatter_hdnLoginId").val();
            var UserId = $("#ConatntMatter_hdnUserId").val();
            var NodeID = $("#ConatntMatter_hdnNodeID").val();

            var data = new FormData();
            data.append("UserID", UserId);
            data.append("NodeID", NodeID);
            data.append("LoginID", LoginId);
            data.append("FileID", "");
            data.append("FileName", ImgName);
            data.append("Actionflg", "9");

            $("#divloader").show();
            $.ajax({
                url: "../../FileUploaderHandler.ashx",
                type: "POST",
                data: data,
                async: true,
                contentType: false,
                processData: false,
                success: function (result) {
                    if (result.split("^")[0] == "0") {
                        $("#fileUploader").val('');
                    }
                    else
                        alert("Error :" + result.split("^")[1]);

                    $("#divloader").hide();
                },
                error: function (err) {
                    alert("Error :" + err.statusText);
                    $("#divloader").hide();
                }
            });
        }
    </script>
    <style type="text/css">
        #btnUpload {
            cursor: pointer;
            font-size: 1rem;
            font-weight: 700;
        }

            #btnUpload:hover {
                text-decoration: underline;
            }

        #divReport {
            overflow-y: auto;
            overflow-x: hidden;
        }

        table.clsChannel > thead > tr > th {
            color: #093D6D;
            font-size: 0.8rem;
            background: #f5f8ff;
            text-align: center;
            vertical-align: middle;
            text-transform: uppercase;
            border-bottom: 2px solid #006092 !important;
        }

        table.clsChannel tr th:nth-child(1) {
            width: 5%;
        }

        table.clsChannel tr th:nth-child(2) {
            width: 20%;
        }

        table.clsChannel tr th:nth-child(4),
        table.clsChannel tr th:nth-child(6),
        table.clsChannel tr th:nth-child(7) {
            width: 15%;
        }

        table.clsChannel tr th:nth-child(3),
        table.clsChannel tr th:nth-child(5),
        table.clsChannel tr th:nth-child(8) {
            width: 10%;
        }


        table.clsChannel td {
            font-size: 0.8rem;
        }

            table.clsChannel td > img {
                cursor: pointer;
            }

            table.clsChannel td > select {
                width: 100%;
            }

        table.clsChannel tr td:nth-child(1),
        table.clsChannel tr td:nth-child(3),
        table.clsChannel tr td:nth-child(5),
        table.clsChannel tr td:nth-child(6),
        table.clsChannel tr td:nth-child(8) {
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
        <div class="ml-4 mt-3">
            <span style="font-size: 1rem; font-weight: 700;">Month-Year : </span>
            <asp:DropDownList ID="ddlMonthYear" runat="server" CssClass="form-control form-control-sm d-inline-block bg-transparent ml-2" onchange="fnShowReport();" Style="width: 200px;"></asp:DropDownList>
            <input id="fileUploader" type="file" onchange="fnUploadImg();" style="visibility: hidden;" />
        </div>
        <div class="mb-2">
        <div class="card-body pb-0" id="divHeader"></div>
        <div class="card-body pt-0" id="divReport"></div>
        </div>
    </div>

    <div id="divloader" class="loader_bg">
        <div class="loader"></div>
    </div>

    <div id="divImage" style="display: none;">
        <div class="text-center">
            <img id="JBPImg" src="../../Files/no-image.png" style="height: 300px;" />
            <div id="btnUpload" class="mt-2" onclick="fnClicktoUploadImg();">Click to Upload</div>
        </div>
    </div>

    <asp:HiddenField ID="hdnUserName" runat="server" />
    <asp:HiddenField ID="hdnLoginId" runat="server" />
    <asp:HiddenField ID="hdnNodeID" runat="server" />
    <asp:HiddenField ID="hdnUserId" runat="server" />

    <asp:HiddenField ID="hdnBranchSubDNodeID" runat="server" />
</asp:Content>

